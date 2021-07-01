using System;

namespace SafeBunny.Core.Serialization
{
    internal interface ISafeBunnySerializer
    {
        public byte[] Serialize<T>(T obj);
        public byte[] Serialize(object obj, Type type);
        public object Deserialize(byte[] obj, Type type);
    }
}