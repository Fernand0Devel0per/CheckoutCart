using TechShop.Helpers.Enums;

namespace TechShop.Dtos.Order
{
    public class OrderSearchResponse
    {
        public Guid Id { get; set; }
        public string OrderDate { get; set; }
        public StatusCode Status { get; set; }
        public Guid UserId { get; set; }
    }
}
