using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order.API.Models;
using Order.API.Models.Entities;
using Order.API.ViewModels;
using Shared.Events;
using Shared.Messages;

namespace Order.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrderDbContext _context;
        private readonly IPublishEndpoint _publishEndpoint;
        public OrdersController(OrderDbContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderVm createOrder)
        {
            Order.API.Models.Entities.Order order = new()
            {
                OrderId = Guid.NewGuid(),
                BuyerId = createOrder.BuyerId,
                OrderStatu = Models.Enums.OrderStatus.Suspend,
                CreatedDate = DateTime.Now,
                TotalPrice = createOrder.CreateOrderItems.Sum(c => c.Price * c.Count)
            };

            order.OrderItems = createOrder.CreateOrderItems.Select(s => new OrderItem
            {
                Id = Guid.NewGuid(),
                ProductId = s.ProductId,
                Price = s.Price,
                Count = s.Count
            }).ToList();

            _context.Orders.Add(order);
            _context.SaveChanges();

            OrderCreatedEvent orderCreatedEvent = new OrderCreatedEvent()
            {
                OrderId = order.OrderId,
                BuyerId = order.BuyerId,
                TotalPrice = order.TotalPrice,
                OrderItems = order.OrderItems
                            .Select(s => new OrderItemMessage
                            {
                                ProductId = s.ProductId,
                                Count = s.Count
                            }).ToList()
            };

            await _publishEndpoint.Publish(orderCreatedEvent);

            return Ok(order);
        }
    }
}
