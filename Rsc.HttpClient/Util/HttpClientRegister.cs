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
using System.Collections.Concurrent;

namespace Rsc.HttpClient.Util
{
    /// <summary>
    /// A threadsafe register for your http clients.
    /// </summary>
    public sealed class HttpClientRegister : IHttpClientRegister
    {
        private bool _disposed = false;
        readonly ConcurrentDictionary<string, IHttpClient> _clients=new ConcurrentDictionary<string, IHttpClient>();
        public void RegisterClient(string serviceName, IHttpClient client)
        {
            _clients.TryAdd(serviceName, client);
        }

        public IHttpClient GetClient(string serviceName)
        {
            IHttpClient client;
            if (_clients.TryGetValue(serviceName, out client))
            {
                return client;
            }
            throw new ArgumentOutOfRangeException(nameof(serviceName));
        }
        void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing) 
            {
                foreach (var client in _clients.Values)
                {
                    try
                    {
                        client.Dispose();
                    }
                    catch (Exception){}
                }
            }
            _clients.Clear();
            _disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~HttpClientRegister()
        {
            Dispose(false);
        }
    }
}