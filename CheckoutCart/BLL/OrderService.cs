using AutoMapper;
using CheckoutCart.BLL.Interface;
using CheckoutCart.DAL;
using CheckoutCart.DAL.Interface;
using CheckoutCart.Domain;
using CheckoutCart.Dtos.Common;
using CheckoutCart.Dtos.Order;
using CheckoutCart.Helpers.Enums;
using CheckoutCart.Helpers.Exceptions;

namespace CheckoutCart.BLL
{
    public class OrderService : IOrderService
    {
        private readonly IOrderDao _orderDao;
        private readonly IUserDao _userDao;
        private readonly IStatusDao _statusDao;
        private readonly IMapper _mapper;


        public OrderService(IConfiguration configuration, IUserDao userDao, IStatusDao statusDao, IMapper mapper)
        {
            _orderDao = new OrderDao(configuration);
            _userDao = userDao;
            _statusDao = statusDao;
            _mapper = mapper;
        }

        public async Task<OrderCreateResponse> CreateAsync(Guid userId)
        {
            if (await _orderDao.DoesUserHaveOpenOrderAsync(userId))
            {
                throw new OpenOrderExistsException("A user cannot have more than one open order.");
            }
            var user = await _userDao.FindByIdAsync(userId);
            if (user == null) 
            {
                throw new UserNotFoundException($"User with Id {userId} not found");
            }
            var statusOpen = await _statusDao.FindByCodeAsync(StatusCode.Open);
            var order = new Order { UserId = userId, StatusId = statusOpen.Id };

            var orderCreated = await _orderDao.CreateAsync(order);
            var orderResponse = _mapper.Map<OrderCreateResponse>(orderCreated);
            orderResponse.Status = StatusCode.Open;
            return orderResponse;
        }

        public async Task<bool> UpdateStatusAsync(Guid id, StatusCode code)
        {
            var order = await _orderDao.GetOrderByIdAsync(id);
            if (order == null)
            {
                throw new OrderNotFoundException($"Order with Id {id} not found");
            }
            var currentStatus = await _statusDao.GetByIdAsync(order.StatusId);

            var newStatus = await _statusDao.FindByCodeAsync(code);
            if (newStatus == null)
            {
                throw new StatusNotFoundException($"Status with Code {code} not found");
            }

            ValidateStatusTransition(currentStatus.Code, newStatus.Code);

            return await _orderDao.UpdateStatusAsync(id, newStatus.Id);
        }

        private void ValidateStatusTransition(int currentStatusCode, int newStatusCode)
        {
            var currentStatus = (StatusCode)currentStatusCode;
            var newStatus = (StatusCode)newStatusCode;

            if (currentStatus == StatusCode.Open && newStatus == StatusCode.Completed)
            {
                throw new InvalidStatusTransitionException("Cannot transition from Open to Completed directly.");
            }
            else if ((currentStatus == StatusCode.CancelledBySystem || currentStatus == StatusCode.CancelledByCustomer) && currentStatus != newStatus)
            {
                throw new InvalidStatusTransitionException("Cannot transition from CancelledBySystem or CancelledByCustomer to another status.");
            }
            else if (currentStatus == StatusCode.AwaitingPayment && newStatus == StatusCode.Open)
            {
                throw new InvalidStatusTransitionException("Cannot transition from AwaitingPayment to Open.");
            }
        }

        public async Task<OrderSearchResponse> GetOrderByIdAsync(Guid id)
        {
            var order = await _orderDao.GetOrderByIdAsync(id);
            if (order == null)
            {
                throw new OrderNotFoundException($"Order with Id {id} not found");
            }

            var status = await _statusDao.GetByIdAsync(order.StatusId);

            var response = _mapper.Map<OrderSearchResponse>(order);
            response.Status = (StatusCode)status.Code;

            return response;
        }

        public Task<Order> GetOrderByIdWithProductsAsync(Guid id)
        {
            return _orderDao.GetOrderByIdAsync(id);
        }

        public Task<PagedResult<Order>> GetOrdersByUserAsync(Guid userId, int page = 1, int pageSize = 10)
        {
            return _orderDao.GetOrdersByUserAsync(userId, page, pageSize);
        }
    }
}
