namespace TechShop.Domain
{
    public class Order
    {
        public Guid Id { get; set; }
        public DateTime OrderDate { get; set; }
        public Guid UserId { get; set; }
        public Guid StatusId { get; set; }
    }
}
