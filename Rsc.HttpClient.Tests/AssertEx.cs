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
    public static class AssertEx
    {
        public static async Task ThrowsAsync<TException>(Func<Task> func)
        {
            var expected = typeof(TException);
            Type actual = null;
            try
            {
                await func();
            }
            catch (Exception e)
            {
                actual = e.GetType();
            }
            Assert.That(expected, Is.EqualTo(actual));
        }
    }
}