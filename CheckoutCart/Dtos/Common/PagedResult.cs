using TechShop.Domain;

namespace TechShop.Dtos.Common
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }

        public PagedResult()
        {
            Items = new List<T>();
        }
    }
}
