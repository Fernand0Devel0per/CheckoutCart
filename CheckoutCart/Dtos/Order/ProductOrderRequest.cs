namespace CheckoutCart.Dtos.Order
{
    public class ProductOrderRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
