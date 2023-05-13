namespace CheckoutCart.Domain
{
    public class Order
    {
        public Guid Id { get; set; }
        public DateTime OrderDate { get; set; }
        public Guid UserId { get; set; } // Foreign Key for User
        public Guid StatusId { get; set; } // Foreign Key for Status
    }
}
