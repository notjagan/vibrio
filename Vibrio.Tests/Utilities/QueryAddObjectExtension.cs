using System.Collections.Specialized;
using System.Reflection;

namespace Vibrio.Tests.Utilities {
    public static class QueryAddObjectExtension {
        public static void AddObject(this NameValueCollection query, object obj, Func<object, string> serialize) {
            var properties = obj.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var property in properties) {
                var value = property.GetValue(obj, null)!;
                if (value is object[] array) {
                    foreach (var item in array) {
                        query.Add(property.Name, serialize(item));
                    }
                } else {
                    query.Add(property.Name, serialize(value));
                }
            }
        }
    }
}
