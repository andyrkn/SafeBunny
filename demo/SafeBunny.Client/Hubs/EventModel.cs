namespace SafeBunny.Client.Hubs
{
    public sealed class EventModel
    {
        public EventModel()
        {
            
        }

        public EventModel(string type, string identity, string content)
        {
            Identity = identity;
            Type = type;
            Content = content;
        }

        public string Identity { get; set; }
        public string Type { get; set; }
        public string Content { get; set; }
    }
}