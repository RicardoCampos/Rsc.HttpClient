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
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Rsc.HttpClient.Tests
{
    class Adding_Headers_Example
    {
        [Test]
        public async Task Add_Headers()
        {
            var guid = "C8B8D9009C1147D3B3BA16443660044F";
            var client=new NoRetryClient(TimeSpan.FromSeconds(30));
            var options = new HttpRequestOptions
            {
                AddHeadersFunc = () =>
                {
                    var header = new KeyValuePair<string, IEnumerable<string>>("x-correlation-id",
                        new[] {guid});

                    return new[] {header};
                }
            };
            var result = await client.GetStringAsync("http://www.google.com",options);
            Assert.IsNotNullOrEmpty(result);
        }
    }
}
