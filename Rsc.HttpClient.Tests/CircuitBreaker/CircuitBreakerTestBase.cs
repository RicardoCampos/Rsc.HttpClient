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
using Moq;
using NUnit.Framework;
using Rsc.HttpClient.Util;

namespace Rsc.HttpClient.Tests.CircuitBreaker
{
    public class CircuitBreakerTestBase
    {
        protected Mock<ITimeService> _timeService;
        protected Rsc.HttpClient.Retry.CircuitBreaker _breaker;

        [SetUp]
        public void Setup()
        {
            _timeService = new Mock<ITimeService>();
            _breaker = new Rsc.HttpClient.Retry.CircuitBreaker(_timeService.Object)
            {
                FailureThreshold = 1,
                CircuitLockout = TimeSpan.FromMinutes(5),
                CircuitRetry = TimeSpan.FromSeconds(1)
            };
        }
    }
}
