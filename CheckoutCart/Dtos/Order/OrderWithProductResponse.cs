using CheckoutCart.Dtos.Product;

namespace CheckoutCart.Dtos.Order
{
    public class OrderWithProductResponse
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public IList<ProductOrderItemResponse> Items { get; set; }
    }
}
