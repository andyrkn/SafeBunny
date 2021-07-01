using System;
using BenchmarkDotNet.Attributes;

namespace SafeBunny.Benchmarks.Serializers
{
    public class SerializerBenchmarks
    {
        private readonly Type protoType = typeof(ProtoModel);
        private readonly Type nonProtoType = typeof(NonProtoModel);
        public ProtoModel protoModel = new("Roxanica", 21, "really smart and nice", 412.231f, 10000);
        public NonProtoModel nonProtoModel = new("Roxanica", 21, "really smart and nice",412.231f, 10000);

        private readonly SafeBunny.Core.Serialization.JsonSerializer _Utf8Serializer = new();
        private readonly SafeBunny.Core.Serialization.JsonSerializer _BinarySerializer = new();
        private readonly SafeBunny.Core.Serialization.JsonSerializer _DotnetJsonSerializer = new();
        private readonly SafeBunny.Core.Serialization.JsonSerializer _NewtonsoftSerializer = new();
        private readonly SafeBunny.Core.Serialization.JsonSerializer _ProtoSerializer = new();

        [Benchmark(Baseline = true)]
        public void Utf8Serializer()
        {
            var res = _Utf8Serializer.Deserialize(_Utf8Serializer.Serialize(nonProtoModel), nonProtoType);
        }

        [Benchmark]
        public void BinarySerializer()
        {
            var res = _BinarySerializer.Deserialize(_Utf8Serializer.Serialize(nonProtoModel), nonProtoType);
        }

        [Benchmark]
        public void DotnetJsonSerializer()
        {
            var res = _DotnetJsonSerializer.Deserialize(_Utf8Serializer.Serialize(nonProtoModel), nonProtoType);
        }

        [Benchmark]
        public void NewtonsoftSerializer()
        {
            var res = _NewtonsoftSerializer.Deserialize(_Utf8Serializer.Serialize(nonProtoModel), nonProtoType);
        }

        [Benchmark]
        public void ProtoSerializer()
        {
            var res = _ProtoSerializer.Deserialize(_Utf8Serializer.Serialize(nonProtoModel), protoType);
        }

    }
}