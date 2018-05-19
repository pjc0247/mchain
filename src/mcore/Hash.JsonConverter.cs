using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

class HashJsonConverter : JsonConverter
{
    public override bool CanRead => true;
    public override bool CanWrite => true;
    public override bool CanConvert(Type objectType) => objectType == typeof(Hash);

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        writer.WriteValue(((Hash)value).str);
    }
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        return new Hash((string)reader.Value);
    }
}