using System;
using System.Collections.Generic;

namespace Proiect_IA.Core
{
    public class Evidence
    {
        private Dictionary<string, string> values;

        public Evidence()
        {
            values = new Dictionary<string, string>();
        }
        public void Set(string nodeName, string value)
        {
            values[nodeName] = value;
        }
        public bool Contains(string nodeName)
        {
            return values.ContainsKey(nodeName);
        }
        public string Get(string nodeName)
        {
            if (!values.ContainsKey(nodeName))
            {
                throw new Exception("Evidence does not contain node: " + nodeName);
            }

            return values[nodeName];
        }
        public Dictionary<string, string> ToDictionary()
        {
            return new Dictionary<string, string>(values);
        }
    }
}
