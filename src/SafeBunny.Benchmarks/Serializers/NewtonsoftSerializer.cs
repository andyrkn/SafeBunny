using System;
using System.Text;
using SafeBunny.Core.Serialization;

namespace SafeBunny.Benchmarks.Serializers
{
    public class NewtonsoftSerializer : ISafeBunnySerializer
    {
        public byte[] Serialize<T>(T obj) => Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(obj));
        public byte[] Serialize(object obj, Type type)
        {
            throw new NotImplementedException();
        }

        public object Deserialize(byte[] obj, Type type) => Newtonsoft.Json.JsonConvert.DeserializeObject(Encoding.UTF8.GetString(obj), type);
    }
}