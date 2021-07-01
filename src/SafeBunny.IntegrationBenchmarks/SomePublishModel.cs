namespace SafeBunny.IntegrationBenchmarks
{
    public class SomePublishModel
    {
        public SomePublishModel() { }
        public SomePublishModel(int messageNumber, string data)
        {
            MessageNumber = messageNumber;
            Data = data;
        }

        public int MessageNumber { get; set; }
        public string Data { get; set; }
    }
}