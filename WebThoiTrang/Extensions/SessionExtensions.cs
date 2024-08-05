using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
namespace WebThoiTrang.Extensions
{
    public static class SessionExtensions
    {
        private static JsonSerializerSettings jsonSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        public static void Set<T>(this ISession session, string key, T value)
            {
                session.SetString(key, JsonConvert.SerializeObject(value));
            }
             public static void SetObjectAsJson(this ISession session, string key, object value)
             {
            session.SetString(key, JsonConvert.SerializeObject(value, jsonSettings));
              }
        public static T Get<T>(this ISession session, string key)
            {
                var value = session.GetString(key);
                return value == null ? default : JsonConvert.DeserializeObject<T>(value);
            }
        
    }
}
