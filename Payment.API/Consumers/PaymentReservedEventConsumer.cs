using MassTransit;
using Shared.Events;

namespace Payment.API.Consumers
{
    public class PaymentReservedEventConsumer : IConsumer<StockReservedEvent>
    {
        public async Task Consume(ConsumeContext<StockReservedEvent> context)
        {
            throw new NotImplementedException();
        }
    }
}
