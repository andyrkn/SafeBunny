namespace SafeBunny.Benchmarks.Serializers
{
    public sealed class NonProtoModel
    {
        public NonProtoModel(string name, int age, string props, float hehexd, double last)
        {
            Name = name;
            Age = age;
            Props = props;
            this.hehexd = hehexd;
            Last = last;
        }
        
        public string Name { get; set; }
        public int Age { get; set; }
        public string Props { get; set; }
        public float hehexd { get; set; }
        public double Last { get; set; }
    }
}