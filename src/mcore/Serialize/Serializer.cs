using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

public static class Serializer
{
    private static JsonConverter[] converters;

    static Serializer()
    {
        // Write custom converters here...
        converters = new JsonConverter[]
        {
            new HashJsonConverter()
        };
    }

    public static string Serialize(this object o, bool indented = false)
    {
        return JsonConvert.SerializeObject(o,
            indented ? Formatting.Indented : Formatting.None,
            converters);
    }
    public static T Deserialize<T>(this string json)
    {
        var setting = new JsonSerializerSettings()
        {
            ContractResolver = new NonPublicPropertiesResolver(),
            Converters = converters
        };

        return JsonConvert.DeserializeObject<T>(json, setting);
    }
}