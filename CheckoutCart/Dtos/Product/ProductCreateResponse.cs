
using TechShop.Helpers.Enums;

namespace TechShop.Dtos.Product
{
    public class ProductCreateResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
    }
}
