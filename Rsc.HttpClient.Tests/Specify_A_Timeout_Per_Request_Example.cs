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

namespace Rsc.HttpClient.Tests
{
    public class Specify_A_Timeout_Per_Request_Example
    {
        [Test]
        public async Task Works_Normally()
        {
            var service1 = new NoRetryClient(TimeSpan.FromSeconds(120));
            var result=await service1.GetStringAsync("http://www.google.com");
            Assert.IsNotNullOrEmpty(result);
        }

        [Test]
        public async Task Will_Timeout_When_Timeout_Passed()
        {
            var service1 = new NoRetryClient(TimeSpan.FromSeconds(120));
            await AssertEx.ThrowsAsync<TaskCanceledException>( async ()=> 
                await service1.GetStringAsync(  "http://www.google.com",
                    new HttpRequestOptions { Timeout = TimeSpan.FromTicks(1) }
                    ));
        }
    }
}