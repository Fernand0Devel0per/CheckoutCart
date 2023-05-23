using TechShop.Dtos.Product;

namespace TechShop.Dtos.Order
{
    public class OrderWithProductResponse
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public IList<ProductOrderItemResponse> Items { get; set; }
    }
}
