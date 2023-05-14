using CheckoutCart.Helpers.Enums;

namespace CheckoutCart.Dtos.Product
{
    public class ProductUpdateRequest
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public CategoryCode Category { get; set; }
        public bool IsActive { get; set; }
    }
}
