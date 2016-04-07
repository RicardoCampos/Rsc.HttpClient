using System;
using System.Collections.Generic;
using System.Linq;

namespace Rsc.HttpClient.Util
{

    public struct Header
    {
        private readonly string _key;
        public string Key => _key;
        private readonly string[] _values;

        public IEnumerable<string> Values => _values;

        public Header(string key, IEnumerable<string> values)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));
            if (values == null) throw new ArgumentNullException(nameof(values));
            _key = key;
            _values = values.ToArray();
        }
    }
}