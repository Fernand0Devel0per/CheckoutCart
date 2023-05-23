namespace TechShop.Domain
{
    public class ProductOrder
    {
        public Guid ProductId { get; set; }
        public Guid OrderId { get; set; }
        public int Quantity { get; set; }
        public decimal PriceAtOrder { get; set; }
    }
}
