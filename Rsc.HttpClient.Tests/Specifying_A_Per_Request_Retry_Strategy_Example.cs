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
using NUnit.Framework;
using Rsc.HttpClient.Retry;

namespace Rsc.HttpClient.Tests
{
    class Specifying_A_Per_Request_Retry_Strategy_Example
    {
        [Test]
        public async Task Specify_Strategy()
        {
            var myService=new RetryClient(TimeSpan.FromSeconds(5), 3);
            var tempStrategy = new NoRetry();
            var options=new HttpRequestOptions
            {
                RetryStrategy = tempStrategy
            };
            var result = await myService.GetStringAsync("http://www.google.com", options);
            Assert.IsNotNullOrEmpty(result);
        }
    }
}