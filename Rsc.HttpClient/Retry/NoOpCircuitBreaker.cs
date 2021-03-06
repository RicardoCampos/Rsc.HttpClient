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

namespace Rsc.HttpClient.Retry
{
    public class NoOpCircuitBreaker : ICircuitBreaker
    {

        public CircuitState State => CircuitState.Closed;


        public TimeSpan CircuitLockout { get; set; }


        public TimeSpan CircuitRetry { get; set; }


        public int FailureThreshold { get; set; }

        public void MarkFailure()
        {
        }

        public void MarkSuccess()
        {
        }

        public bool AllowRequest()
        {
            return true;
        }

        public void Reset()
        {
        }

    }
}