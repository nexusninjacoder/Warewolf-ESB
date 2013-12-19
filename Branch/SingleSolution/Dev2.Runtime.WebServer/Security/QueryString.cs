using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.SignalR.Hosting;

namespace Dev2.Runtime.WebServer.Security
{
    public class QueryString : INameValueCollection
    {
        readonly IEnumerable<KeyValuePair<string, string>> _items;

        public QueryString(IEnumerable<KeyValuePair<string, string>> items)
        {
            VerifyArgument.IsNotNull("items", items);
            _items = items;
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<string> GetValues(string key)
        {
            var value = this[key];
            if(string.IsNullOrEmpty(value))
            {
                return new string[0];
            }

            var values = value.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            for(var i = 0; i < values.Length; i++)
            {
                values[i] = values[i].Trim();
            }
            return values;
        }

        public string Get(string key)
        {
            return this[key];
        }

        public string this[string key]
        {
            get
            {
                var item = _items.FirstOrDefault(i => string.Compare(i.Key, key, StringComparison.InvariantCultureIgnoreCase) == 0);
                return string.IsNullOrEmpty(item.Value) ? string.Empty : item.Value;
            }
        }
    }
}