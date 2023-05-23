using AutoMapper;
using CheckoutCart.BLL.Interface;
using CheckoutCart.DAL.Interface;
using CheckoutCart.Domain;
using CheckoutCart.Dtos.Common;
using CheckoutCart.Dtos.Order;
using CheckoutCart.Dtos.Product;
using CheckoutCart.Helpers.Enums;
using CheckoutCart.Helpers.Exceptions;

namespace CheckoutCart.BLL
{
    public class OrderService : IOrderService
    {
        private readonly IOrderDao _orderDao;
        private readonly IUserDao _userDao;
        private readonly IProductDao _productDao;
        private readonly IProductOrderDao _productOrderDao;
        private readonly IStatusDao _statusDao;
        private readonly IMapper _mapper;


        public OrderService(IConfiguration configuration, IUserDao userDao, IStatusDao statusDao, IMapper mapper, IOrderDao orderDao, IProductOrderDao productOrderDao, IProductDao productDao)
        {
            _userDao = userDao;
            _statusDao = statusDao;
            _mapper = mapper;
            _orderDao = orderDao;
            _productOrderDao = productOrderDao;
            _productDao = productDao;
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

        public async Task<OrderWithProductResponse> GetOrderByIdWithProductsAsync(Guid id)
        {
            var order = await _orderDao.GetOrderByIdAsync(id);
            if (order == null)
            {
                throw new OrderNotFoundException($"Order with Id {id} not found");
            }
            var itemsOrder = await _productOrderDao.GetAllProductsInOrderAsync(order.Id);

            var itemsOrderResponse = await MappingProductsResponse(itemsOrder);

            var orderResponse = _mapper.Map<OrderWithProductResponse>(order);
            orderResponse.Items = itemsOrderResponse;
            return orderResponse;
        }

        public async Task<PagedResult<OrderSearchResponse>> GetOrdersByUserAsync(Guid userId, int page = 1, int pageSize = 10)
        {
            var orderResult = await _orderDao.GetOrdersByUserAsync(userId, page, pageSize);

            var result = new PagedResult<OrderSearchResponse>
            {
                TotalPages = orderResult.TotalPages,
                TotalItems = orderResult.TotalItems,
            };

            foreach (var order in orderResult.Items)
            {
                var status = await _statusDao.GetByIdAsync(order.StatusId); 
                var mappedOrder = _mapper.Map<OrderSearchResponse>(order);
                mappedOrder.Status = (StatusCode)status.Code;

                result.Items.Add(mappedOrder);
            }

            return result;
        }



        private async Task<IList<ProductOrderItemResponse>> MappingProductsResponse(IEnumerable<ProductOrder> items)
        {
            var itemsOrderResponse = new List<ProductOrderItemResponse>();
            if (items.Count() > 0)
            {
                foreach (var item in items)
                {
                    var itemResponse = _mapper.Map<ProductOrderItemResponse>(item);
                    var product = await _productDao.GetProductByIdAsync(item.ProductId);
                    itemResponse.Name = product.Name;
                    itemsOrderResponse.Add(itemResponse);
                }
            }
            return itemsOrderResponse;
        }

        public async Task<bool> AddProductToOrderAsync(Guid orderId, Guid productId, int quantity)
        {
          
            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero");
            }

            var product = await _productDao.GetProductByIdAsync(productId);
            if (product == null)
            {
                throw new ProductNotFoundException($"Product with Id {productId} not found");
            }

            if (!product.IsActive)
            {
                throw new ArgumentException("The product is not active");
            }

            var order = await _orderDao.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                throw new OrderNotFoundException($"Order with Id {orderId} not found");
            }
            var status = await _statusDao.GetByIdAsync(order.StatusId);
            if (status.Code != (int)StatusCode.Open)
            {
                throw new InvalidOrderStatusException("Order is not open, cannot add product");
            }

            var productOrder = new ProductOrder { OrderId = orderId, ProductId = productId, Quantity = quantity, PriceAtOrder = product.Price };

            return await _productOrderDao.AddProductToOrderAsync(productOrder);
        }

        public async Task<bool> UpdateProductQuantityInOrderAsync(Guid orderId, Guid productId, int newQuantity)
        {
            var order = await _orderDao.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                throw new OrderNotFoundException($"Order with Id {orderId} not found");
            }

            var status = await _statusDao.GetByIdAsync(order.StatusId);
            if (status.Code != (int)StatusCode.Open)
            {
                throw new InvalidOrderStatusException("Order is not open, cannot update product quantity");
            }

            var productOrder = await _productOrderDao.GetProductOrderAsync(orderId, productId);
            if (productOrder == null)
            {
                throw new ProductNotFoundException($"Product with OrderId {orderId} and ProductId {productId} not found");
            }

            if (newQuantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(newQuantity), "Quantity must be greater than zero");
            }

            return await _productOrderDao.UpdateProductQuantityInOrderAsync(orderId, productId, newQuantity);
        }

        public async Task<bool> RemoveProductFromOrderAsync(Guid orderId, Guid productId)
        {
            var order = await _orderDao.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                throw new OrderNotFoundException($"Order with Id {orderId} not found");
            }

            var status = await _statusDao.GetByIdAsync(order.StatusId);
            if (status.Code != (int)StatusCode.Open)
            {
                throw new InvalidOrderStatusException("Order is not open, cannot remove product");
            }

            var productOrder = await _productOrderDao.GetProductOrderAsync(orderId, productId);
            if (productOrder == null)
            {
                throw new ProductNotFoundException($"Product with OrderId {orderId} and ProductId {productId} not found");
            }

            return await _productOrderDao.RemoveProductFromOrderAsync(orderId, productId);
        }
    }
}
