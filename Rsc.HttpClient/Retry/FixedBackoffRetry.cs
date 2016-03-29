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

namespace Rsc.HttpClient.Retry
{
    public class FixedBackoffRetry : AlwaysAllowsRequestsBase, IRetryStrategy
    {
        private readonly int _maxRetries;
        private readonly TimeSpan _backoffTime;

        public FixedBackoffRetry(int maxRetries, int backoffInMilliseconds)
        {
            _maxRetries = maxRetries;
            _backoffTime = TimeSpan.FromMilliseconds(backoffInMilliseconds);
        }

        public async Task<T> Execute<T>(Func<Task<T>> taskToExecute)
        {
            return await DoExecute(taskToExecute, 0);
        }

        async Task<T> DoExecute<T>(Func<Task<T>> taskToExecute, int retryCount)
        {
            try
            {
                return await taskToExecute.Invoke()
                    .ConfigureAwait(false);
            }
            catch (Exception)
            {
                retryCount++;
                if (retryCount > _maxRetries) throw;
                await Task.Delay(_backoffTime);
                await DoExecute(taskToExecute, retryCount);
            }
            throw new InvalidOperationException();
        }
    }
}