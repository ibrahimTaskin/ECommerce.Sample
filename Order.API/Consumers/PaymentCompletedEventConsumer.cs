using MassTransit;
using Order.API.Models;
using Shared.Events;

namespace Order.API.Consumers
{
    public class PaymentCompletedEventConsumer : IConsumer<PaymentCompletedEvent>
    {
        readonly OrderDbContext _ordererContext;
        public PaymentCompletedEventConsumer(OrderDbContext ordererContext)
        {
            _ordererContext = ordererContext;
        }
        public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
        {
            var order = _ordererContext.Orders.FirstOrDefault(o => o.OrderId == context.Message.OrderId);
            order.OrderStatu = Models.Enums.OrderStatus.Comleted;
            await _ordererContext.SaveChangesAsync();
        }
    }
}
