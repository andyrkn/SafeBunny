using System;
using SafeBunny.Core.Serialization;

namespace SafeBunny.Benchmarks.Serializers
{
    internal sealed class BinarySerializer : ISafeBunnySerializer
    {
        private readonly BinarySerializer Serializer = new ();

        public byte[] Serialize<T>(T obj) => Serializer.Serialize<T>(obj);
        public byte[] Serialize(object obj, Type type)
        {
            throw new NotImplementedException();
        }

        public object Deserialize(byte[] obj, Type type) => Serializer.Deserialize(obj, type);
    }
}