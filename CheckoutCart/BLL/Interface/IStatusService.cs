using TechShop.Dtos.Status;

namespace TechShop.BLL.Interface
{
    public interface IStatusService
    {
        Task<IEnumerable<StatusResponse>> GetAllStatusAsync();
    }
}
