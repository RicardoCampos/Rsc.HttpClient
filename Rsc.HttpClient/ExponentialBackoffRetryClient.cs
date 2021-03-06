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
using System.Net.Http;
using Rsc.HttpClient.Retry;

namespace Rsc.HttpClient
{
    public sealed class ExponentialBackoffRetryClient : NoRetryClient
    {
        public override IRetryStrategy RetryStrategy { get; }
        public ExponentialBackoffRetryClient(TimeSpan timeout, int maxRetries, int backoffInMilliseconds) 
            : this(timeout, new HttpClientHandler(), maxRetries,backoffInMilliseconds)
        {

        }

        public ExponentialBackoffRetryClient(TimeSpan timeout, HttpMessageHandler httpMessageHandler, int maxRetries, int backoffInMilliseconds) 
            : this(timeout, httpMessageHandler, true, maxRetries,backoffInMilliseconds)
        {
        }

        public ExponentialBackoffRetryClient(TimeSpan timeout, HttpMessageHandler httpMessageHandler, bool disposeHandler, int maxRetries, int backoffInMilliseconds) 
            : base(timeout, httpMessageHandler, disposeHandler)
        {
            RetryStrategy = new ExponentialBackoffRetry(maxRetries,backoffInMilliseconds);
        }
    }
}