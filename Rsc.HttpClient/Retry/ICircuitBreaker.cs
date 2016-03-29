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
    public interface ICircuitBreaker: IShouldCheckBeforeSubmittingRequests
    {
        /// <summary>
        /// The current state of the circuit. this should be used for reporting only and not used to make decisions.
        /// Use AllowRequest to determine whether you should attempt to use the circuit or not.
        /// </summary>
        CircuitState State { get; }
        
        /// <summary>
        /// The amount of time the circuit is broken for when the failure threshold has been reached.
        /// </summary>
        TimeSpan CircuitLockout { get; set; }
        
        /// <summary>
        /// The amount of time the circuit will allow befoer retrying in order to see if the circuit is closed again.
        /// If a circuit is broken it will allow the occasional request to check whether the circuit is working again 
        /// (otherwise a circuit will be broken forever).
        /// </summary>
        TimeSpan CircuitRetry { get; set; }
        
        /// <summary>
        /// The number of failures tolerated before the circuit is considered broken.
        /// </summary>
        int FailureThreshold { get; set; }

        /// <summary>
        /// Used to mark failure when an operation fails. If the circuit is broken this should trigger a CircuitBrokenException. 
        /// </summary>
        void MarkFailure();

        /// <summary>
        /// Used to mark success when an operation succeeds. This may open the circuit.
        /// </summary>
        void MarkSuccess();

        /// <summary>
        /// Resets the state of the circuit, assuming it is closed (working) and resets all timers.
        /// </summary>
        void Reset();
    }
}