using System;
using System.Collections.Generic;
using Rsc.HttpClient.Retry;

namespace Rsc.HttpClient
{
    /// <summary>
    /// Specifies options that override class defaults for a single request.
    /// </summary>
    public class HttpRequestOptions
    {
        /// <summary>
        /// The retry strategy to be used for this request. If left null, the default strategy for the IHttpClient derived class will be used.
        /// </summary>
        public IRetryStrategy RetryStrategy { get; set; }

        /// <summary>
        /// The timeout to be used for this request. If left null, the default timeout for the IHttpClient derived class will be used.
        /// </summary>
        public TimeSpan? Timeout { get; set; }

        /// <summary>
        /// A function returning a collections of headers to be added to the request.
        /// A header is represented by a name and a collection of string values for that name.
        /// </summary>
        public Func<IEnumerable<KeyValuePair<string,IEnumerable<string>>>> AddHeadersFunc { get; set; } 
    }

}