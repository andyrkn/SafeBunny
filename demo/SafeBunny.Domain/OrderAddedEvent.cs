namespace SafeBunny.Domain
{
    public sealed class OrderAddedEvent
    {
        public OrderAddedEvent(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}