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
using Rsc.HttpClient.Util;

namespace Rsc.HttpClient.Tests
{
    class Adding_Headers_Example
    {
        const string GUID = "C8B8D9009C1147D3B3BA16443660044F";

        [Test]
        public async Task Add_Headers()
        {
            var client=new NoRetryClient(TimeSpan.FromSeconds(30));
            var options = new HttpRequestOptions
            {
                AddHeadersFunc = () => new[] {new Header("x-correlation-id",new[] { GUID }) }
            };
            var result = await client.GetStringAsync("http://www.google.com",options);
            Assert.IsNotNullOrEmpty(result);
        }

        [Test]
        public async Task Add_Headers_Using_Factory()
        {           
            var client = new NoRetryClient(TimeSpan.FromSeconds(30));
            var options = new HttpRequestOptions
            {
                AddHeadersFunc = () => CorrelationHeaderFactory.Create(GUID)
            };
            var result = await client.GetStringAsync("http://www.google.com", options);
            Assert.IsNotNullOrEmpty(result);
        }

        /// <summary>
        /// This could just as easily be from an interface and injected in.
        /// </summary>
        private static class CorrelationHeaderFactory
        {
            private const string Key = "x-correlation-id";
            public static IEnumerable<Header> Create(string value)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return CreateHeader(Guid.NewGuid().ToString());
                }
                return CreateHeader(value);
            }

            private static IEnumerable<Header> CreateHeader(string value)
            {
                return new[] {new Header(Key, new[] {value})};
            }
        }
    }
}
