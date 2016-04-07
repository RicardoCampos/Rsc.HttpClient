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
    public interface IHttpClient : IDisposable
    {
        IRetryStrategy RetryStrategy { get; }

        bool AllowRequest();
        /// <summary>
        /// Gets the headers which should be sent with each request.
        /// </summary>
        /// 
        /// <returns>
        /// Returns <see cref="T:System.Net.Http.Headers.HttpRequestHeaders"/>.The headers which should be sent with each request.
        /// </returns>

        HttpRequestHeaders DefaultRequestHeaders { get; }

        /// <summary>
        /// Gets or sets the base address of Uniform Resource Identifier (URI) of the Internet resource used when sending requests.
        /// </summary>
        /// 
        /// <returns>
        /// Returns <see cref="T:System.Uri"/>.The base address of Uniform Resource Identifier (URI) of the Internet resource used when sending requests.
        /// </returns>

        Uri BaseAddress { get; set; }

        /// <summary>
        /// Gets or sets the timespan to wait before the request times out.
        /// </summary>
        /// 
        /// <returns>
        /// Returns <see cref="T:System.TimeSpan"/>.The timespan to wait before the request times out.
        /// </returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The timeout specified is less than or equal to zero and is not <see cref="F:System.Threading.Timeout.InfiniteTimeSpan"/>.</exception><exception cref="T:System.InvalidOperationException">An operation has already been started on the current instance. </exception><exception cref="T:System.ObjectDisposedException">The current instance has been disposed.</exception>

        TimeSpan Timeout { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of bytes to buffer when reading the response content.
        /// </summary>
        /// 
        /// <returns>
        /// Returns <see cref="T:System.Int32"/>.The maximum number of bytes to buffer when reading the response content. The default value for this property is 2 gigabytes.
        /// </returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The size specified is less than or equal to zero.</exception><exception cref="T:System.InvalidOperationException">An operation has already been started on the current instance. </exception><exception cref="T:System.ObjectDisposedException">The current instance has been disposed. </exception>

        long MaxResponseContentBufferSize { get; set; }


        /// <summary>
        /// Send a GET request to the specified Uri and return the response body as a string in an asynchronous operation.
        /// </summary>
        /// 
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task`1"/>.The task object representing the asynchronous operation.
        /// </returns>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="requestOptions">The retry strategy</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri"/> was null.</exception>
        Task<string> GetStringAsync(string requestUri, HttpRequestOptions requestOptions =null);

        /// <summary>
        /// Send a GET request to the specified Uri and return the response body as a string in an asynchronous operation.
        /// </summary>
        /// 
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task`1"/>.The task object representing the asynchronous operation.
        /// </returns>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="requestOptions"></param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri"/> was null.</exception>
        Task<string> GetStringAsync(Uri requestUri, HttpRequestOptions requestOptions = null);

        /// <summary>
        /// Send a GET request to the specified Uri and return the response body as a byte array in an asynchronous operation.
        /// </summary>
        /// 
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task`1"/>.The task object representing the asynchronous operation.
        /// </returns>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="requestOptions"></param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri"/> was null.</exception>
        Task<byte[]> GetByteArrayAsync(string requestUri, HttpRequestOptions requestOptions = null);

        /// <summary>
        /// Send a GET request to the specified Uri and return the response body as a byte array in an asynchronous operation.
        /// </summary>
        /// 
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task`1"/>.The task object representing the asynchronous operation.
        /// </returns>
        /// <param name="requestUri">The Uri the request is sent to.</param><exception cref="T:System.ArgumentNullException">The <paramref name="requestUri"/> was null.</exception>

        Task<byte[]> GetByteArrayAsync(Uri requestUri, HttpRequestOptions requestOptions = null);

        /// <summary>
        /// Send a GET request to the specified Uri and return the response body as a stream in an asynchronous operation.
        /// </summary>
        /// 
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task`1"/>.The task object representing the asynchronous operation.
        /// </returns>
        /// <param name="requestUri">The Uri the request is sent to.</param><exception cref="T:System.ArgumentNullException">The <paramref name="requestUri"/> was null.</exception>

        Task<Stream> GetStreamAsync(string requestUri, HttpRequestOptions requestOptions = null);

        /// <summary>
        /// Send a GET request to the specified Uri and return the response body as a stream in an asynchronous operation.
        /// </summary>
        /// 
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task`1"/>.The task object representing the asynchronous operation.
        /// </returns>
        /// <param name="requestUri">The Uri the request is sent to.</param><exception cref="T:System.ArgumentNullException">The <paramref name="requestUri"/> was null.</exception>

        Task<Stream> GetStreamAsync(Uri requestUri, HttpRequestOptions requestOptions = null);



        /// <summary>
        /// Send a GET request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// 
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task`1"/>.The task object representing the asynchronous operation.
        /// </returns>
        /// <param name="requestUri">The Uri the request is sent to.</param><exception cref="T:System.ArgumentNullException">The <paramref name="requestUri"/> was null.</exception>

        Task<HttpResponseMessage> GetAsync(string requestUri, HttpRequestOptions requestOptions = null);

        /// <summary>
        /// Send a GET request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// 
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task`1"/>.The task object representing the asynchronous operation.
        /// </returns>
        /// <param name="requestUri">The Uri the request is sent to.</param><exception cref="T:System.ArgumentNullException">The <paramref name="requestUri"/> was null.</exception>

        Task<HttpResponseMessage> GetAsync(Uri requestUri, HttpRequestOptions requestOptions = null);

        /// <summary>
        /// Send a GET request to the specified Uri with an HTTP completion option as an asynchronous operation.
        /// </summary>
        /// 
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task`1"/>.
        /// </returns>
        /// <param name="requestUri">The Uri the request is sent to.</param><param name="completionOption">An HTTP completion option value that indicates when the operation should be considered completed.</param><exception cref="T:System.ArgumentNullException">The <paramref name="requestUri"/> was null.</exception>

        Task<HttpResponseMessage> GetAsync(string requestUri, HttpCompletionOption completionOption, HttpRequestOptions requestOptions = null);

        /// <summary>
        /// Send a GET request to the specified Uri with an HTTP completion option as an asynchronous operation.
        /// </summary>
        /// 
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task`1"/>.The task object representing the asynchronous operation.
        /// </returns>
        /// <param name="requestUri">The Uri the request is sent to.</param><param name="completionOption">An HTTP  completion option value that indicates when the operation should be considered completed.</param><exception cref="T:System.ArgumentNullException">The <paramref name="requestUri"/> was null.</exception>

        Task<HttpResponseMessage> GetAsync(Uri requestUri, HttpCompletionOption completionOption, HttpRequestOptions requestOptions = null);

        /// <summary>
        /// Send a GET request to the specified Uri with a cancellation token as an asynchronous operation.
        /// </summary>
        /// 
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task`1"/>.
        /// </returns>
        /// <param name="requestUri">The Uri the request is sent to.</param><param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param><exception cref="T:System.ArgumentNullException">The <paramref name="requestUri"/> was null.</exception>

        Task<HttpResponseMessage> GetAsync(string requestUri, CancellationToken cancellationToken, HttpRequestOptions requestOptions = null);


        /// <summary>
        /// Send a GET request to the specified Uri with a cancellation token as an asynchronous operation.
        /// </summary>
        /// 
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task`1"/>.The task object representing the asynchronous operation.
        /// </returns>
        /// <param name="requestUri">The Uri the request is sent to.</param><param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param><exception cref="T:System.ArgumentNullException">The <paramref name="requestUri"/> was null.</exception>

        Task<HttpResponseMessage> GetAsync(Uri requestUri, CancellationToken cancellationToken, HttpRequestOptions requestOptions = null);

        /// <summary>
        /// Send a GET request to the specified Uri with an HTTP completion option and a cancellation token as an asynchronous operation.
        /// </summary>
        /// 
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task`1"/>.
        /// </returns>
        /// <param name="requestUri">The Uri the request is sent to.</param><param name="completionOption">An HTTP  completion option value that indicates when the operation should be considered completed.</param><param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param><exception cref="T:System.ArgumentNullException">The <paramref name="requestUri"/> was null.</exception>

        Task<HttpResponseMessage> GetAsync(string requestUri, HttpCompletionOption completionOption,
            CancellationToken cancellationToken, HttpRequestOptions requestOptions = null);

        /// <summary>
        /// Send a GET request to the specified Uri with an HTTP completion option and a cancellation token as an asynchronous operation.
        /// </summary>
        /// 
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task`1"/>.The task object representing the asynchronous operation.
        /// </returns>
        /// <param name="requestUri">The Uri the request is sent to.</param><param name="completionOption">An HTTP  completion option value that indicates when the operation should be considered completed.</param><param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param><exception cref="T:System.ArgumentNullException">The <paramref name="requestUri"/> was null.</exception>

        Task<HttpResponseMessage> GetAsync(Uri requestUri, HttpCompletionOption completionOption,
            CancellationToken cancellationToken, HttpRequestOptions requestOptions = null);

        /// <summary>
        /// Send a POST request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// 
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task`1"/>.The task object representing the asynchronous operation.
        /// </returns>
        /// <param name="requestUri">The Uri the request is sent to.</param><param name="content">The HTTP request content sent to the server.</param><exception cref="T:System.ArgumentNullException">The <paramref name="requestUri"/> was null.</exception>

        Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content, HttpRequestOptions requestOptions = null);

        /// <summary>
        /// Send a POST request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// 
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task`1"/>.The task object representing the asynchronous operation.
        /// </returns>
        /// <param name="requestUri">The Uri the request is sent to.</param><param name="content">The HTTP request content sent to the server.</param><exception cref="T:System.ArgumentNullException">The <paramref name="requestUri"/> was null.</exception>

        Task<HttpResponseMessage> PostAsync(Uri requestUri, HttpContent content, HttpRequestOptions requestOptions = null);

        /// <summary>
        /// Send a POST request with a cancellation token as an asynchronous operation.
        /// </summary>
        /// 
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task`1"/>.The task object representing the asynchronous operation.
        /// </returns>
        /// <param name="requestUri">The Uri the request is sent to.</param><param name="content">The HTTP request content sent to the server.</param><param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param><exception cref="T:System.ArgumentNullException">The <paramref name="requestUri"/> was null.</exception>
        Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content,
            CancellationToken cancellationToken, HttpRequestOptions requestOptions = null);

        /// <summary>
        /// Send a POST request with a cancellation token as an asynchronous operation.
        /// </summary>
        /// 
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task`1"/>.The task object representing the asynchronous operation.
        /// </returns>
        /// <param name="requestUri">The Uri the request is sent to.</param><param name="content">The HTTP request content sent to the server.</param><param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param><exception cref="T:System.ArgumentNullException">The <paramref name="requestUri"/> was null.</exception>

        Task<HttpResponseMessage> PostAsync(Uri requestUri, HttpContent content,
            CancellationToken cancellationToken, HttpRequestOptions requestOptions = null);

        /// <summary>
        /// Send a PUT request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// 
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task`1"/>.The task object representing the asynchronous operation.
        /// </returns>
        /// <param name="requestUri">The Uri the request is sent to.</param><param name="content">The HTTP request content sent to the server.</param><exception cref="T:System.ArgumentNullException">The <paramref name="requestUri"/> was null.</exception>
        Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content, HttpRequestOptions requestOptions = null);

        /// <summary>
        /// Send a PUT request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// 
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task`1"/>.The task object representing the asynchronous operation.
        /// </returns>
        /// <param name="requestUri">The Uri the request is sent to.</param><param name="content">The HTTP request content sent to the server.</param><exception cref="T:System.ArgumentNullException">The <paramref name="requestUri"/> was null.</exception>

        Task<HttpResponseMessage> PutAsync(Uri requestUri, HttpContent content, HttpRequestOptions requestOptions = null);

        /// <summary>
        /// Send a PUT request with a cancellation token as an asynchronous operation.
        /// </summary>
        /// 
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task`1"/>.The task object representing the asynchronous operation.
        /// </returns>
        /// <param name="requestUri">The Uri the request is sent to.</param><param name="content">The HTTP request content sent to the server.</param><param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param><exception cref="T:System.ArgumentNullException">The <paramref name="requestUri"/> was null.</exception>
        Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content,
            CancellationToken cancellationToken, HttpRequestOptions requestOptions = null);

        /// <summary>
        /// Send a PUT request with a cancellation token as an asynchronous operation.
        /// </summary>
        /// 
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task`1"/>.The task object representing the asynchronous operation.
        /// </returns>
        /// <param name="requestUri">The Uri the request is sent to.</param><param name="content">The HTTP request content sent to the server.</param><param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param><exception cref="T:System.ArgumentNullException">The <paramref name="requestUri"/> was null.</exception>
        Task<HttpResponseMessage> PutAsync(Uri requestUri, HttpContent content,
            CancellationToken cancellationToken, HttpRequestOptions requestOptions = null);

        /// <summary>
        /// Send a DELETE request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// 
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task`1"/>.The task object representing the asynchronous operation.
        /// </returns>
        /// <param name="requestUri">The Uri the request is sent to.</param><exception cref="T:System.ArgumentNullException">The <paramref name="requestUri"/> was null.</exception><exception cref="T:System.InvalidOperationException">The request message was already sent by the <see cref="T:System.Net.Http.HttpClient"/> instance.</exception>
        Task<HttpResponseMessage> DeleteAsync(string requestUri, HttpRequestOptions requestOptions = null);

        /// <summary>
        /// Send a DELETE request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// 
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task`1"/>.The task object representing the asynchronous operation.
        /// </returns>
        /// <param name="requestUri">The Uri the request is sent to.</param><exception cref="T:System.ArgumentNullException">The <paramref name="requestUri"/> was null.</exception><exception cref="T:System.InvalidOperationException">The request message was already sent by the <see cref="T:System.Net.Http.HttpClient"/> instance.</exception>
        Task<HttpResponseMessage> DeleteAsync(Uri requestUri, HttpRequestOptions requestOptions = null);


        /// <summary>
        /// Send a DELETE request to the specified Uri with a cancellation token as an asynchronous operation.
        /// </summary>
        /// 
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task`1"/>.The task object representing the asynchronous operation.
        /// </returns>
        /// <param name="requestUri">The Uri the request is sent to.</param><param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param><exception cref="T:System.ArgumentNullException">The <paramref name="requestUri"/> was null.</exception><exception cref="T:System.InvalidOperationException">The request message was already sent by the <see cref="T:System.Net.Http.HttpClient"/> instance.</exception>
        Task<HttpResponseMessage> DeleteAsync(string requestUri, CancellationToken cancellationToken, HttpRequestOptions requestOptions = null);

        /// <summary>
        /// Send a DELETE request to the specified Uri with a cancellation token as an asynchronous operation.
        /// </summary>
        /// 
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task`1"/>.The task object representing the asynchronous operation.
        /// </returns>
        /// <param name="requestUri">The Uri the request is sent to.</param><param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param><exception cref="T:System.ArgumentNullException">The <paramref name="requestUri"/> was null.</exception><exception cref="T:System.InvalidOperationException">The request message was already sent by the <see cref="T:System.Net.Http.HttpClient"/> instance.</exception>
        Task<HttpResponseMessage> DeleteAsync(Uri requestUri, CancellationToken cancellationToken, HttpRequestOptions requestOptions = null);

        /// <summary>
        /// Send an HTTP request as an asynchronous operation.
        /// </summary>
        /// 
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task`1"/>.The task object representing the asynchronous operation.
        /// </returns>
        /// <param name="request">The HTTP request message to send.</param><exception cref="T:System.ArgumentNullException">The <paramref name="request"/> was null.</exception><exception cref="T:System.InvalidOperationException">The request message was already sent by the <see cref="T:System.Net.Http.HttpClient"/> instance.</exception>

        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpRequestOptions requestOptions = null);

        /// <summary>
        /// Send an HTTP request as an asynchronous operation.
        /// </summary>
        /// 
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task`1"/>.The task object representing the asynchronous operation.
        /// </returns>
        /// <param name="request">The HTTP request message to send.</param><param name="cancellationToken">The cancellation token to cancel operation.</param><exception cref="T:System.ArgumentNullException">The <paramref name="request"/> was null.</exception><exception cref="T:System.InvalidOperationException">The request message was already sent by the <see cref="T:System.Net.Http.HttpClient"/> instance.</exception>
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken, HttpRequestOptions requestOptions = null);

        /// <summary>
        /// Send an HTTP request as an asynchronous operation.
        /// </summary>
        /// 
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task`1"/>.The task object representing the asynchronous operation.
        /// </returns>
        /// <param name="request">The HTTP request message to send.</param><param name="completionOption">When the operation should complete (as soon as a response is available or after reading the whole response content).</param><exception cref="T:System.ArgumentNullException">The <paramref name="request"/> was null.</exception><exception cref="T:System.InvalidOperationException">The request message was already sent by the <see cref="T:System.Net.Http.HttpClient"/> instance.</exception>
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, HttpRequestOptions requestOptions = null);

        /// <summary>
        /// Send an HTTP request as an asynchronous operation.
        /// </summary>
        /// 
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task`1"/>.The task object representing the asynchronous operation.
        /// </returns>
        /// <param name="request">The HTTP request message to send.</param><param name="completionOption">When the operation should complete (as soon as a response is available or after reading the whole response content).</param><param name="cancellationToken">The cancellation token to cancel operation.</param><exception cref="T:System.ArgumentNullException">The <paramref name="request"/> was null.</exception><exception cref="T:System.InvalidOperationException">The request message was already sent by the <see cref="T:System.Net.Http.HttpClient"/> instance.</exception>
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption,
            CancellationToken cancellationToken, HttpRequestOptions requestOptions = null);

        /// <summary>
        /// Cancel all pending requests on this instance.
        /// </summary>
        void CancelPendingRequests();



    }
}