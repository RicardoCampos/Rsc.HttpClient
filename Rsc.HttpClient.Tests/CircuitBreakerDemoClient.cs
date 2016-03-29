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
using System.Diagnostics;
using System.Threading.Tasks;
using NUnit.Framework;
using Rsc.HttpClient.Retry;
using Rsc.HttpClient.Util;

namespace Rsc.HttpClient.Tests
{
    /// <summary>
    /// This class isn't a unit test, but rather an alternative to haveing a console application
    /// to demonstrate using the library.
    /// </summary>
    class CircuitBreakerDemoClient
    {
        private IHttpClient _googleHttpClient;
        private ICircuitBreaker _googleCircuitBreaker;

        [SetUp]
        public void Setup()
        {
            var timeService = new TimeService();
            _googleCircuitBreaker = new Retry.CircuitBreaker(timeService);
            _googleHttpClient = new CircuitBreakerClient(TimeSpan.FromSeconds(1),_googleCircuitBreaker);
        }

        [Test]
        public void You_Can_Alter_Lockout_Times_For_Your_Circuit()
        {
            _googleCircuitBreaker.CircuitLockout=TimeSpan.FromMinutes(1);
            _googleCircuitBreaker.CircuitRetry=TimeSpan.FromSeconds(30);
            Assert.IsTrue(true);
        }

        [Test]
        public void Check_Before_You_Try_A_Request()
        {
            Assert.Throws<CircuitBrokenException>(() => _googleCircuitBreaker.MarkFailure());
            if (!_googleHttpClient.AllowRequest())
            {
                Assert.IsTrue(true);
                Debug.WriteLine("If you check, you can avoid expensive timeouts or exceptions.");
            }
        }

        [Test]
        public async Task Make_A_Request()
        {
            if (_googleHttpClient.AllowRequest())
            {
                var something = await _googleHttpClient.GetStringAsync("http://www.google.com");
                Assert.IsNotEmpty(something);
            }
        }

        [Test]
        public async Task If_You_Do_not_Check_For_Broken_Circuit_The_Adapter_Will_Not_Either_And_Will_Throw_Exception()
        {
            Assert.Throws<CircuitBrokenException>(()=>_googleCircuitBreaker.MarkFailure());
            Assert.IsFalse(_googleCircuitBreaker.AllowRequest());
            Assert.IsFalse(_googleHttpClient.AllowRequest(), "This should be the same as checking the circuit breaker directly.");
            await AssertEx.ThrowsAsync<CircuitBrokenException>(async ()=>await _googleHttpClient.GetStringAsync("http://www.google.com"));
        }
    }
}
