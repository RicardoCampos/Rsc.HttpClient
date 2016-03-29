/**
  Copyright 2016 Ricardo Campos

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
   **/

using System;
using System.Threading;
using Rsc.HttpClient.Util;

namespace Rsc.HttpClient.Retry
{
    /// <summary>
    /// The default circuit breaker implementation
    /// </summary>
    public class CircuitBreaker : ICircuitBreaker
    {
        private readonly ITimeService _timeService;
        private int _failureThreshold=1;
        private int _failures;
        private DateTime _brokenTime;
        private readonly SemaphoreSlim _lock=new SemaphoreSlim(1);
        public CircuitBreaker(ITimeService timeService)
        {
            if (timeService == null) throw new ArgumentNullException(nameof(timeService));
            _timeService = timeService;
            CircuitLockout=TimeSpan.FromMinutes(5);
            CircuitRetry=TimeSpan.FromMinutes(1);
        }

        public CircuitState State { get; private set; } = CircuitState.Closed;

        public TimeSpan CircuitLockout { get; set; }

        public TimeSpan CircuitRetry { get; set; }

        public int FailureThreshold
        {
            get { return _failureThreshold; }
            set
            {

                _failureThreshold = value;
            }
        }

        public void MarkFailure()
        {
            Interlocked.Increment(ref _failures);
            try
            {
                _lock.Wait();
                if (_failures < _failureThreshold || State == CircuitState.Open) return;
                //break the circuit
                State = CircuitState.Open;
                _brokenTime = _timeService.GetCurrentUtcDateTime();
            }
            finally
            {
                _lock.Release();
            }
  
            throw new CircuitBrokenException();
        }

        public void MarkSuccess()
        {
            try
            {
                _lock.Wait();
                if (State == CircuitState.Open)
                {
                    ResetUnsafe();
                }
            }
            finally
            {
                _lock.Release();
            }
        }

        public bool AllowRequest()
        {
            try
            {
                _lock.Wait();
                switch (State)
                {
                    case CircuitState.Closed:
                    case CircuitState.Partial:
                        return true;
                    case CircuitState.Open:
                        var now = _timeService.GetCurrentUtcDateTime();
                        var timeSinceBroken = now - _brokenTime;
                        if (timeSinceBroken >= CircuitRetry)
                        {
                            _brokenTime = now;
                            return true;
                        }
                        return false;
                    default:
                        return false;
                }
            }
            finally
            {
                _lock.Release();
            }
        }

        public void Reset()
        {
            try
            {
                _lock.Wait();
                ResetUnsafe();
            }
            finally
            {
                _lock.Release();
            }
        }

        private void ResetUnsafe()
        {
            _failures = 0;
            State = CircuitState.Closed;
        }
    }
}