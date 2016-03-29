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
using System.Threading.Tasks;
using Rsc.HttpClient.Util;

namespace Rsc.HttpClient.Retry
{
    /// <summary>
    /// Strategy that defers retry logic to it's internal circuit breaker.
    /// If the circuit is broken, it will throw a CircuitBrokenException.
    /// </summary>
    /// <remarks>
    /// The circuit breaker instance should be a singleton otherwise it doesn't make sense to use this strategy.
    /// Since the circuit isn't typed, you can share a single circuit between different retry strategies.
    /// </remarks>
    /// <typeparam name="T">The type to return</typeparam>
    public class CircuitBreakerStrategy : IRetryStrategy
    {
        private readonly ICircuitBreaker _circuitBreaker;

        public CircuitBreakerStrategy(ICircuitBreaker circuitBreaker)
        {
            if (circuitBreaker == null) throw new ArgumentNullException(nameof(circuitBreaker));
            _circuitBreaker = circuitBreaker;
        }

        public async Task<T> Execute<T>(Func<Task<T>> taskToExecute)
        {
            return await DoExecute(taskToExecute);
        }

        private async Task<T> DoExecute<T>(Func<Task<T>> taskToExecute)
        {
            try
            {
                if(!_circuitBreaker.AllowRequest()) throw new CircuitBrokenException();
                var result= await taskToExecute.Invoke()
                                               .ConfigureAwait(false);
                _circuitBreaker.MarkSuccess();
                return result;
            }
            catch (Exception e)
            {
                _circuitBreaker.MarkFailure();
                if (_circuitBreaker.AllowRequest())
                {
                    return await DoExecute(taskToExecute);
                }
                throw new CircuitBrokenException("Request not allowed as circuit has been broken.",e);
            }
        }

        public bool AllowRequest()
        {
            return _circuitBreaker.AllowRequest();
        }
    }
}