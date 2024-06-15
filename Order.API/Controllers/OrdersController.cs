using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order.API.Models;
using Order.API.Models.Entities;
using Order.API.ViewModels;

namespace Order.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrderDbContext _context;
        public OrdersController(OrderDbContext context)
        {
           _context = context;
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
                TotalPrice = createOrder.CreateOrderItems.Sum(c => c.Count)
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

            return Ok(order);
        }
    }
}
