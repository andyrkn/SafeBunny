using System;
using SafeBunny.Core.Serialization;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SafeBunny.Benchmarks.Serializers
{
    internal sealed class dotnetSerializer : ISafeBunnySerializer
    {
        public byte[] Serialize<T>(T obj) => JsonSerializer.SerializeToUtf8Bytes(obj);
        public byte[] Serialize(object obj, Type type)
        {
            throw new NotImplementedException();
        }

        public object Deserialize(byte[] obj, Type type) => JsonSerializer.Deserialize(obj, type);
    }
}