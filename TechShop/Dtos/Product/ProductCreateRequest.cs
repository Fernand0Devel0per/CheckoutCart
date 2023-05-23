using TechShop.Helpers.Enums;

namespace TechShop.Dtos.Product
{
    public class ProductCreateRequest
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public CategoryCode Category { get; set; }
        public bool IsActive { get; set; }
    }
}
