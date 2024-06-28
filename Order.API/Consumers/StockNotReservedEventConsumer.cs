using MassTransit;
using Order.API.Models;
using Shared.Events;

namespace Order.API.Consumers
{
    public class StockNotReservedEventConsumer : IConsumer<StockNotReservedEvent>
    {
        private readonly OrderDbContext _dbContext;
        public StockNotReservedEventConsumer(OrderDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task Consume(ConsumeContext<StockNotReservedEvent> context)
        {
            var order = _dbContext.Orders.Where(w => w.OrderId == context.Message.OrderId && w.BuyerId == context.Message.BuyerId).FirstOrDefault();
            order.OrderStatu = Models.Enums.OrderStatus.Failed;
            await _dbContext.SaveChangesAsync();
        }
    }
}
