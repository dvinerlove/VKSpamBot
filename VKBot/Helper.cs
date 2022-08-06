using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKBot
{
    public static class Helper
    {
        public static string ToJson(this object source)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(source);
        }

        public static T? ToObject<T>(this string source)
        {
            try
            {
                return (T?)Newtonsoft.Json.JsonConvert.DeserializeObject(source, typeof(T));
            }
            catch
            {
                return default(T);
            }
        }
    }
}
