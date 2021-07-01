namespace SafeBunny.Domain
{
    public sealed class InvoiceCreatedEvent
    {
        public InvoiceCreatedEvent(int price)
        {
            Price = price;
        }

        public int Price { get; set; }
    }
}