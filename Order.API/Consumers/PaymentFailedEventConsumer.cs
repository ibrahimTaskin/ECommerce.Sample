using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Models;
using Shared.Events;

namespace Order.API.Consumers
{
    public class PaymentFailedEventConsumer : IConsumer<PaymentFailedEvent>
    {
        private readonly OrderDbContext _context;
        public PaymentFailedEventConsumer(OrderDbContext context)
        {
            _context = context;
        }
        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(f => f.OrderId == context.Message.OrderId);
            order.OrderStatu = Models.Enums.OrderStatus.Failed;
            await _context.SaveChangesAsync();
        }
    }
}
