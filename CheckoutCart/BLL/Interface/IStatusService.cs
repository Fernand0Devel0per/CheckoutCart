using CheckoutCart.Dtos.Status;

namespace CheckoutCart.BLL.Interface
{
    public interface IStatusService
    {
        Task<IEnumerable<StatusResponse>> GetAllStatusAsync();
    }
}
