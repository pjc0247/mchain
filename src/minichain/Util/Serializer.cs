﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace minichain
{
    internal static class Serializer
    {
        public static string Serialize(this object o, bool indented = false)
        {
            return JsonConvert.SerializeObject(o,
                indented ? Formatting.Indented : Formatting.None);
        }
        public static T Deserialize<T>(this string json)
        {
            var setting = new JsonSerializerSettings()
            {
                ContractResolver = new NonPublicPropertiesResolver()
            };

            return JsonConvert.DeserializeObject<T>(json, setting);
        }
    }
}