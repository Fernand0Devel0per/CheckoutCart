using AutoMapper;
using CheckoutCart.BLL.Interface;
using CheckoutCart.DAL.Interface;
using CheckoutCart.Dtos.Status;

namespace CheckoutCart.BLL
{
    public class StatusService : IStatusService
    {
        private readonly IStatusDao _statusDao;
        private readonly IMapper _mapper;

        public StatusService(IStatusDao statusDao, IMapper mapper)
        {
            _statusDao = statusDao;
            _mapper = mapper;
        }

        public async Task<IEnumerable<StatusResponse>> GetAllStatusAsync()
        {
            var allStatus = await _statusDao.FindAllAsync();
            return _mapper.Map<IEnumerable<StatusResponse>>(allStatus);
        }
    }
}
