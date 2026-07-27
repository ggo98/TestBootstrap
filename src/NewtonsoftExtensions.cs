using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestBootstrap
{
    public static class NewtonsoftExtensions
    {
        public static T FromJson<T>(this string json)
        {
            T ret = JsonConvert.DeserializeObject<T>(json);
            return ret;
        }
    }
}