using System;
using System.IO;
using ProtoBuf;
using SafeBunny.Core.Serialization;

namespace SafeBunny.Benchmarks.Serializers
{
    internal sealed class ProtoSerializer : ISafeBunnySerializer
    {
        public byte[] Serialize<T>(T obj)
        {
            var stream = new MemoryStream();
            Serializer.Serialize(stream, obj);
            return stream.ToArray();
        }

        public byte[] Serialize(object obj, Type type)
        {
            throw new NotImplementedException();
        }

        public object Deserialize(byte[] obj, Type type)
        {
            return Serializer.Deserialize(obj.AsSpan(), type);
        }
    }

    [ProtoContract]
    public sealed class ProtoModel
    {
        public ProtoModel(string name, int age, string props, float hehexd, double last)
        {
            Name = name;
            Age = age;
            Props = props;
            this.hehexd = hehexd;
            Last = last;
        }

        [ProtoMember(1)]
        public string Name { get; set; }
        [ProtoMember(2)]
        public int Age { get; set; }
        [ProtoMember(3)]
        public string Props { get; set; }
        [ProtoMember(4)]
        public float hehexd { get; set; }
        [ProtoMember(5)]
        public double Last { get; set; }
    }
}