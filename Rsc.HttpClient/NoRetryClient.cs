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
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Rsc.HttpClient.Retry;

namespace Rsc.HttpClient
{
    public class NoRetryClient : IHttpClient
    {
        private bool _disposed = false;
        private System.Net.Http.HttpClient _client;

        #region Constructors
        public NoRetryClient(TimeSpan timeout):this(timeout,new HttpClientHandler())
        {

        }

        public NoRetryClient(TimeSpan timeout,HttpMessageHandler httpMessageHandler):this(timeout,httpMessageHandler,true)
        {
        }

        public NoRetryClient(TimeSpan timeout,HttpMessageHandler httpMessageHandler, bool disposeHandler)
        {
            RetryStrategy = new NoRetry();
            _client = new System.Net.Http.HttpClient(httpMessageHandler, disposeHandler)
            {
                Timeout = timeout
            };
        }
        #endregion

        #region Disposal
        void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                if (_client == null) return;
                _client.CancelPendingRequests();
                _client.Dispose();
                _client = null;
            }
            _disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~NoRetryClient()
        {
            Dispose(false);
        }
        #endregion

        public virtual IRetryStrategy RetryStrategy { get; }

        public virtual bool AllowRequest()
        {
            return RetryStrategy.AllowRequest();
        }

        public HttpRequestHeaders DefaultRequestHeaders => _client.DefaultRequestHeaders;

        public Uri BaseAddress
        {
            get { return _client.BaseAddress; }
            set { _client.BaseAddress = value; }
        }

        public TimeSpan Timeout
        {
            get { return _client.Timeout; }
            set { _client.Timeout = value; }
        }

        public long MaxResponseContentBufferSize
        {
            get { return _client.MaxResponseContentBufferSize; }
            set { _client.MaxResponseContentBufferSize = value; }
        }

        #region GET

        public Task<string> GetStringAsync(string requestUri, IRetryStrategy retryStrategy = null)
        {
            return GetStringAsync(new Uri(requestUri),retryStrategy);
        }

        public async Task<string> GetStringAsync(Uri requestUri, IRetryStrategy retryStrategy = null)
        {
            if (retryStrategy == null)
            {
                return await RetryStrategy.Execute(() => _client.GetStringAsync(requestUri));
            }
            return await retryStrategy.Execute(() => _client.GetStringAsync(requestUri));
        }

        public Task<byte[]> GetByteArrayAsync(string requestUri, IRetryStrategy retryStrategy=null )
        {
            return GetByteArrayAsync(new Uri(requestUri),retryStrategy);
        }

        public async Task<byte[]> GetByteArrayAsync(Uri requestUri,IRetryStrategy retryStrategy= null)
        {
            if (retryStrategy == null)
            {
                return await RetryStrategy.Execute(() => _client.GetByteArrayAsync(requestUri));
            }
            return await retryStrategy.Execute(() => _client.GetByteArrayAsync(requestUri));
        }

        public Task<Stream> GetStreamAsync(string requestUri, IRetryStrategy retryStrategy=null )
        {
            return GetStreamAsync(new Uri(requestUri),retryStrategy);
        }

        public async Task<Stream> GetStreamAsync(Uri requestUri, IRetryStrategy retryStrategy = null)
        {
            if (retryStrategy == null)
            {
                return await RetryStrategy.Execute(() => _client.GetStreamAsync(requestUri));
            }
            return await retryStrategy.Execute(() => _client.GetStreamAsync(requestUri));
        }

        public Task<HttpResponseMessage> GetAsync(string requestUri,IRetryStrategy retryStrategy=null)
        {
            return GetAsync(new Uri(requestUri), retryStrategy);
        }

        public Task<HttpResponseMessage> GetAsync(Uri requestUri, IRetryStrategy retryStrategy = null)
        {
            return GetAsync(requestUri, HttpCompletionOption.ResponseContentRead, new CancellationToken(), retryStrategy);
        }

        public Task<HttpResponseMessage> GetAsync(string requestUri, HttpCompletionOption completionOption, IRetryStrategy retryStrategy = null)
        {
            return GetAsync(new Uri(requestUri), completionOption, retryStrategy);
        }

        public Task<HttpResponseMessage> GetAsync(Uri requestUri, HttpCompletionOption completionOption, IRetryStrategy retryStrategy = null)
        {
            return GetAsync(requestUri,completionOption,new CancellationToken(), retryStrategy);
        }

        public Task<HttpResponseMessage> GetAsync(string requestUri, CancellationToken cancellationToken, IRetryStrategy retryStrategy = null)
        {
            return GetAsync(new Uri(requestUri), cancellationToken, retryStrategy);
        }

        public Task<HttpResponseMessage> GetAsync(Uri requestUri, CancellationToken cancellationToken, IRetryStrategy retryStrategy = null)
        {
            return GetAsync(requestUri,HttpCompletionOption.ResponseContentRead, cancellationToken, retryStrategy);
        }

        public Task<HttpResponseMessage> GetAsync(string requestUri, HttpCompletionOption completionOption, CancellationToken cancellationToken, IRetryStrategy retryStrategy = null)
        {
            return GetAsync(new Uri(requestUri), completionOption, cancellationToken,retryStrategy);
        }

        public async Task<HttpResponseMessage> GetAsync(Uri requestUri, HttpCompletionOption completionOption, CancellationToken cancellationToken, IRetryStrategy retryStrategy = null)
        {
            if (retryStrategy == null)
            {
                return
                    await
                        RetryStrategy.Execute(
                            () => _client.GetAsync(requestUri, completionOption, cancellationToken));
            }
            return
                await
                    retryStrategy.Execute(
                        () => _client.GetAsync(requestUri, completionOption, cancellationToken));
        }
        #endregion

        #region POST
        public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content, IRetryStrategy retryStrategy = null)
        {
            return PostAsync(new Uri(requestUri), content,retryStrategy);
        }

        public Task<HttpResponseMessage> PostAsync(Uri requestUri, HttpContent content, IRetryStrategy retryStrategy = null)
        {
            return PostAsync(requestUri, content, new CancellationToken(),retryStrategy);

        }

        public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content, CancellationToken cancellationToken, IRetryStrategy retryStrategy = null)
        {
            return PostAsync(new Uri(requestUri), content, cancellationToken,retryStrategy);
        }

        public async Task<HttpResponseMessage> PostAsync(Uri requestUri, HttpContent content, CancellationToken cancellationToken, IRetryStrategy retryStrategy = null)
        {
            if (retryStrategy == null)
            {
                return
                    await
                        RetryStrategy.Execute(
                            () => _client.PostAsync(requestUri, content, cancellationToken));
            }
            return
               await
                   retryStrategy.Execute(
                       () => _client.PostAsync(requestUri, content, cancellationToken));
        }
        #endregion

        #region PUT
        public Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content, IRetryStrategy retryStrategy = null)
        {
            return PutAsync(new Uri(requestUri), content,retryStrategy);
        }

        public Task<HttpResponseMessage> PutAsync(Uri requestUri, HttpContent content, IRetryStrategy retryStrategy = null)
        {
            return PutAsync(requestUri,content, new CancellationToken(),retryStrategy);
        }

        public Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content, CancellationToken cancellationToken, IRetryStrategy retryStrategy = null)
        {
            return PutAsync(new Uri(requestUri), content, cancellationToken,retryStrategy);
        }

        public async Task<HttpResponseMessage> PutAsync(Uri requestUri, HttpContent content, CancellationToken cancellationToken, IRetryStrategy retryStrategy = null)
        {
            if (retryStrategy == null)
            {
                return
                    await
                        RetryStrategy.Execute(
                            () => _client.PutAsync(requestUri, content, cancellationToken));
            }
            return await retryStrategy.Execute(() => _client.PutAsync(requestUri, content, cancellationToken));
        }
        #endregion

        #region DELETE
        public Task<HttpResponseMessage> DeleteAsync(string requestUri, IRetryStrategy retryStrategy = null)
        {
            return DeleteAsync(new Uri(requestUri),retryStrategy);
        }

        public Task<HttpResponseMessage> DeleteAsync(Uri requestUri, IRetryStrategy retryStrategy = null)
        {
            return DeleteAsync(requestUri, new CancellationToken(),retryStrategy);
        }

        public Task<HttpResponseMessage> DeleteAsync(string requestUri, CancellationToken cancellationToken, IRetryStrategy retryStrategy = null)
        {
            return DeleteAsync(new Uri(requestUri), cancellationToken,retryStrategy);
        }

        public async Task<HttpResponseMessage> DeleteAsync(Uri requestUri, CancellationToken cancellationToken, IRetryStrategy retryStrategy = null)
        {
            if (retryStrategy == null)
            {
                return await RetryStrategy.Execute(() => _client.DeleteAsync(requestUri, cancellationToken));
            }
            return await retryStrategy.Execute(() => _client.DeleteAsync(requestUri, cancellationToken));
        }
        #endregion

        #region Send
        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, IRetryStrategy retryStrategy=null )
        {
            return SendAsync(request, new CancellationToken(),retryStrategy);
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken, IRetryStrategy retryStrategy = null)
        {
            return SendAsync(request,HttpCompletionOption.ResponseContentRead ,cancellationToken,retryStrategy);
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, IRetryStrategy retryStrategy = null)
        {
            return SendAsync(request, completionOption, new CancellationToken(),retryStrategy);
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken, IRetryStrategy retryStrategy = null)
        {
            if (retryStrategy == null)
            {
                return await RetryStrategy.Execute(() => _client.SendAsync(request, completionOption, cancellationToken));
            }
            return await retryStrategy.Execute(() => _client.SendAsync(request, completionOption, cancellationToken));
        }
        #endregion

        public void CancelPendingRequests()
        {
            _client.CancelPendingRequests();
        }
    }
}