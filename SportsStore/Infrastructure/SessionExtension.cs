using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
//using System.Text.Json;
using System.Threading.Tasks;

namespace SportsStore.Infrastructure
{
    public static class SessionExtension
    {
        public static void SetJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetJson<T>(this ISession session, string key)
        {
            var sessionData = session.GetString(key);

            return sessionData == null
                ? default : JsonConvert.DeserializeObject<T>(sessionData);
        }
    }
}
