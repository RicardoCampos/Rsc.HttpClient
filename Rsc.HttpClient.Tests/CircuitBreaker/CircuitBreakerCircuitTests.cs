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
using NUnit.Framework;
using Rsc.HttpClient.Retry;
using Rsc.HttpClient.Util;

namespace Rsc.HttpClient.Tests.CircuitBreaker
{
    [TestFixture]
    public class CircuitBreakerCircuitTests : CircuitBreakerTestBase
    {

        [Test]
        public void Breaker_Starts_Closed()
        {
            Assert.IsTrue(_breaker.AllowRequest());
        }

        [Test]
        public void Failures_Above_Threshold_Breaks_Circuit()
        {
            _timeService.Setup(t => t.GetCurrentUtcDateTime())
                .Returns(new DateTime(2015, 1, 1, 8, 0, 0));
            Assert.Throws<CircuitBrokenException>(() => _breaker.MarkFailure());
            Assert.IsFalse(_breaker.AllowRequest());
        }

        [Test]
        public void After_Retry_Timespan_Elapsed_Will_Allow_One_Request()
        {
            _timeService.Setup(t => t.GetCurrentUtcDateTime())
                .Returns(new DateTime(2015, 1, 1, 8, 0, 0));
            Assert.Throws<CircuitBrokenException>(()=>_breaker.MarkFailure());
            _timeService.Setup(t => t.GetCurrentUtcDateTime())
                .Returns(new DateTime(2015, 1, 1, 8, 0, 5));
            Assert.IsTrue(_breaker.AllowRequest());
            Assert.IsFalse(_breaker.AllowRequest());
        }

        [Test]
        public void Circuit_Reopens_After_A_Success()
        {
            _timeService.Setup(t => t.GetCurrentUtcDateTime())
                .Returns(new DateTime(2015, 1, 1, 8, 0, 0));
            Assert.Throws<CircuitBrokenException>(() => _breaker.MarkFailure());
            Assert.That(_breaker.State, Is.EqualTo(CircuitState.Open));
            _breaker.MarkSuccess();
            Assert.That(_breaker.State, Is.EqualTo(CircuitState.Closed));
            Assert.IsTrue(_breaker.AllowRequest());
            Assert.IsTrue(_breaker.AllowRequest());
        }
    }
    
}