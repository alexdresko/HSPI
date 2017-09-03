using System.Collections.Generic;
using System.Linq;

namespace GutenTag
{
    internal static class StringPairs
    {
        public static IEnumerable<KeyValuePair<string, string>> FromObject(object obj)
        {
            if (obj == null)
            {
                return Enumerable.Empty<KeyValuePair<string, string>>();
            }

            return obj.GetType()
                .GetProperties()
                .Where(p => p.PropertyType == typeof(string))
                .Select(p => new KeyValuePair<string, string>(p.Name, p.GetValue(obj, null) as string));
        }
    }
}