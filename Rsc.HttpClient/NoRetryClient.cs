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
using Rsc.HttpClient.Util;

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

        public Task<string> GetStringAsync(string requestUri, HttpRequestOptions requestOptions = null)
        {
            return GetStringAsync(new Uri(requestUri),requestOptions);
        }

        public Task<string> GetStringAsync(Uri requestUri, HttpRequestOptions requestOptions = null)
        {
            return GetContentAsync(requestUri, HttpCompletionOption.ResponseContentRead, string.Empty, 
                content => content.ReadAsStringAsync(),requestOptions);
        }

        public Task<byte[]> GetByteArrayAsync(string requestUri, HttpRequestOptions requestOptions =null )
        {
            return GetByteArrayAsync(new Uri(requestUri),requestOptions);
        }

        public Task<byte[]> GetByteArrayAsync(Uri requestUri, HttpRequestOptions requestOptions = null)
        {
            return GetContentAsync(requestUri, HttpCompletionOption.ResponseContentRead, HttpUtilities.EmptyByteArray, 
                content => content.ReadAsByteArrayAsync(),requestOptions);
        }

        public Task<Stream> GetStreamAsync(string requestUri, HttpRequestOptions requestOptions =null )
        {
            return GetStreamAsync(new Uri(requestUri),requestOptions);
        }

        public Task<Stream> GetStreamAsync(Uri requestUri, HttpRequestOptions requestOptions = null)
        {
            return GetContentAsync(requestUri, HttpCompletionOption.ResponseHeadersRead, Stream.Null, content =>
            {
                if (content == null) throw new ArgumentNullException(nameof(content));
                return content.ReadAsStreamAsync();
            },requestOptions);
        }


        private Task<T> GetContentAsync<T>(Uri requestUri, HttpCompletionOption completionOption, T defaultValue,
            Func<HttpContent, Task<T>> readAs,HttpRequestOptions requestOptions=null)
        {
            var tcs = new TaskCompletionSource<T>();
            
            GetAsync(requestUri, completionOption,requestOptions).ContinueWithStandard(requestTask =>
            {
                if (HandleRequestFaultsAndCancelation<T>(requestTask, tcs))
                    return;
                var result = requestTask.Result;
                if (result.Content == null)
                {
                    tcs.TrySetResult(defaultValue);
                }
                else
                {
                    try
                    {
                        HttpUtilities.ContinueWithStandard(readAs(result.Content), contentTask =>
                        {
                            if (HttpUtilities.HandleFaultsAndCancelation(contentTask, tcs)) return;
                            tcs.TrySetResult(contentTask.Result);
                        });
                    }
                    catch (Exception ex)
                    {
                        tcs.TrySetException(ex);
                    }
                }
            });
            return tcs.Task;
        }

        private static bool HandleRequestFaultsAndCancelation<T>(Task<HttpResponseMessage> task, TaskCompletionSource<T> tcs)
        {
            if (HttpUtilities.HandleFaultsAndCancelation(task, tcs)) return true;
            var result = task.Result;
            if (result.IsSuccessStatusCode) return false;
            result.Content?.Dispose();
            tcs.TrySetException(new HttpRequestException($"Status Code: {result.StatusCode} Reason: {result.ReasonPhrase}"));
            return true;
        }    

        #region HttpResponseMessage Gets

        public Task<HttpResponseMessage> GetAsync(string requestUri, HttpRequestOptions requestOptions =null)
        {
            return GetAsync(new Uri(requestUri), requestOptions);
        }

        public Task<HttpResponseMessage> GetAsync(Uri requestUri, HttpRequestOptions requestOptions = null)
        {
            return GetAsync(requestUri, HttpCompletionOption.ResponseContentRead, CancellationToken.None, requestOptions);
        }

        public Task<HttpResponseMessage> GetAsync(string requestUri, HttpCompletionOption completionOption, HttpRequestOptions requestOptions = null)
        {
            return GetAsync(new Uri(requestUri), completionOption, requestOptions);
        }

        public Task<HttpResponseMessage> GetAsync(Uri requestUri, HttpCompletionOption completionOption, HttpRequestOptions requestOptions = null)
        {
            return GetAsync(requestUri,completionOption,CancellationToken.None, requestOptions);
        }

        public Task<HttpResponseMessage> GetAsync(string requestUri, CancellationToken cancellationToken, HttpRequestOptions requestOptions = null)
        {
            return GetAsync(new Uri(requestUri), cancellationToken, requestOptions);
        }

        public Task<HttpResponseMessage> GetAsync(Uri requestUri, CancellationToken cancellationToken, HttpRequestOptions requestOptions = null)
        {
            return GetAsync(requestUri,HttpCompletionOption.ResponseContentRead, cancellationToken, requestOptions);
        }

        public Task<HttpResponseMessage> GetAsync(string requestUri, HttpCompletionOption completionOption, CancellationToken cancellationToken, HttpRequestOptions requestOptions = null)
        {
            return GetAsync(new Uri(requestUri), completionOption, cancellationToken,requestOptions);
        }

        public Task<HttpResponseMessage> GetAsync(Uri requestUri, HttpCompletionOption completionOption, CancellationToken cancellationToken, HttpRequestOptions requestOptions = null)
        {
            return SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri), completionOption, cancellationToken,requestOptions);
        }
        #endregion
        
        #endregion

        #region POST
        public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content, HttpRequestOptions requestOptions = null)
        {
            return PostAsync(new Uri(requestUri), content,requestOptions);
        }

        public Task<HttpResponseMessage> PostAsync(Uri requestUri, HttpContent content, HttpRequestOptions requestOptions = null)
        {
            return PostAsync(requestUri, content, CancellationToken.None,requestOptions);

        }

        public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content, CancellationToken cancellationToken, HttpRequestOptions requestOptions = null)
        {
            return PostAsync(new Uri(requestUri), content, cancellationToken,requestOptions);
        }

        public Task<HttpResponseMessage> PostAsync(Uri requestUri, HttpContent content, CancellationToken cancellationToken, HttpRequestOptions requestOptions = null)
        {
            return SendAsync(new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = content
            }, 
            cancellationToken,
            requestOptions);
        }
        #endregion

        #region PUT
        public Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content, HttpRequestOptions requestOptions = null)
        {
            return PutAsync(new Uri(requestUri), content,requestOptions);
        }

        public Task<HttpResponseMessage> PutAsync(Uri requestUri, HttpContent content, HttpRequestOptions requestOptions = null)
        {
            return PutAsync(requestUri,content, CancellationToken.None,requestOptions);
        }

        public Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content, CancellationToken cancellationToken, HttpRequestOptions requestOptions = null)
        {
            return PutAsync(new Uri(requestUri), content, cancellationToken,requestOptions);
        }

        public async Task<HttpResponseMessage> PutAsync(Uri requestUri, HttpContent content, CancellationToken cancellationToken, HttpRequestOptions requestOptions = null)
        {
            return await SendAsync(new HttpRequestMessage(HttpMethod.Put, requestUri)
            {
                Content = content
            }, cancellationToken);
        }
        #endregion

        #region DELETE
        public Task<HttpResponseMessage> DeleteAsync(string requestUri, HttpRequestOptions requestOptions = null)
        {
            return DeleteAsync(new Uri(requestUri),requestOptions);
        }

        public Task<HttpResponseMessage> DeleteAsync(Uri requestUri, HttpRequestOptions requestOptions = null)
        {
            return DeleteAsync(requestUri, CancellationToken.None,requestOptions);
        }

        public Task<HttpResponseMessage> DeleteAsync(string requestUri, CancellationToken cancellationToken, HttpRequestOptions requestOptions = null)
        {
            return DeleteAsync(new Uri(requestUri), cancellationToken,requestOptions);
        }

        public Task<HttpResponseMessage> DeleteAsync(Uri requestUri, CancellationToken cancellationToken, HttpRequestOptions requestOptions = null)
        {
            return SendAsync(new HttpRequestMessage(HttpMethod.Delete, requestUri), cancellationToken,requestOptions);
        }
        #endregion

        #region Send
        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpRequestOptions requestOptions =null )
        {
            return SendAsync(request, CancellationToken.None,requestOptions);
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken, HttpRequestOptions requestOptions = null)
        {
            return SendAsync(request,HttpCompletionOption.ResponseContentRead ,cancellationToken,requestOptions);
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, HttpRequestOptions requestOptions = null)
        {
            return SendAsync(request, completionOption, CancellationToken.None,requestOptions);
        }


        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken, HttpRequestOptions requestOptions = null)
        {
            if (request == null) throw new ArgumentNullException("request");
            if (requestOptions == null)
            {
                return RetryStrategy.Execute(() => _client.SendAsync(request, completionOption, cancellationToken));
            }
            if (requestOptions.AddHeadersFunc != null)
            {
                var headers = requestOptions.AddHeadersFunc.Invoke();
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }
            if (requestOptions.Timeout == null)
            {
                return RetryStrategy.Execute(() => _client.SendAsync(request, completionOption, cancellationToken));
            }
            if (requestOptions.RetryStrategy != null)
            {
                return requestOptions.RetryStrategy.Execute(() => SendAsyncWithTimeout(request, completionOption, requestOptions.Timeout.Value));
            }
            return RetryStrategy.Execute(() => SendAsyncWithTimeout(request, completionOption, requestOptions.Timeout.Value));
        }

        private Task<HttpResponseMessage> SendAsyncWithTimeout(HttpRequestMessage request, HttpCompletionOption completionOption, TimeSpan timeout)
        {
            var source = new CancellationTokenSource();
            var task = _client.SendAsync(request, completionOption, source.Token);
            source.CancelAfter(timeout);
            return task;
        }
        #endregion

        public void CancelPendingRequests()
        {
            _client.CancelPendingRequests();
        }
    }
}