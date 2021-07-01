using System;

namespace SafeBunny.Core.Serialization
{
    internal sealed class JsonSerializer : ISafeBunnySerializer
    {
        public byte[] Serialize<T>(T obj) 
            => Utf8Json.JsonSerializer.Serialize(obj);

        public byte[] Serialize(object obj, Type type)
            => Utf8Json.JsonSerializer.NonGeneric.Serialize(type, obj);

        public object Deserialize(byte[] obj, Type type) 
            => Utf8Json.JsonSerializer.NonGeneric.Deserialize(type, obj);
    }
}