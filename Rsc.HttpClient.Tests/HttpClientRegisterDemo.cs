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
using Rsc.HttpClient.Util;

namespace Rsc.HttpClient.Tests
{
    class HttpClientRegisterDemo
    {

        [Test]
        public void Use_Register_In_Your_Bootstrap_Process()
        {
            
            var service1=new NoRetryClient(TimeSpan.FromSeconds(1));
            var service2 = new NoRetryClient(TimeSpan.FromSeconds(1));
            var circuit = new Retry.CircuitBreaker(new TimeService());
            var service3=new CircuitBreakerClient(TimeSpan.FromSeconds(1),circuit);
            
            // To get ultimate benefit, register the HttpClientRegister as a singleton.
            IHttpClientRegister register=new HttpClientRegister();
            register.RegisterClient("Google",service1);
            register.RegisterClient("Bing",service2);
            register.RegisterClient("DuckDuckGo",service3);

            Assert.That(register.GetClient("Google"), Is.SameAs(service1));
            Assert.That(register.GetClient("Bing"), Is.SameAs(service2));
            Assert.That(register.GetClient("DuckDuckGo"), Is.SameAs(service3));
        }
    }
}
