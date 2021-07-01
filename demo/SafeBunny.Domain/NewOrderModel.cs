namespace SafeBunny.Domain
{
    public sealed class NewOrderModel
    {
        public NewOrderModel(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
