using MassTransit;
using Shared.Events;

namespace Payment.API.Consumers
{
    public class StockReservedEventConsumer : IConsumer<StockReservedEvent>
    {

        readonly IPublishEndpoint _publishEndpoint;
        public StockReservedEventConsumer(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }
        public async Task Consume(ConsumeContext<StockReservedEvent> context)
        {
            Random random = new();
            int a = random.Next(1, 10);

            if (a % 2 == 0) // Ödeme başarılı
            {
                PaymentCompletedEvent paymentCompletedEvent = new()
                {
                    OrderId = context.Message.OrderId
                };
                await _publishEndpoint.Publish(paymentCompletedEvent);
                await Console.Out.WriteLineAsync("Ödeme başarılı.");
            }
            else // Ödemede sıkıntı var.
            {
                PaymentFailedEvent paymentFailedEvent = new()
                {
                    OrderId = context.Message.OrderId,
                    Message = "Ödeme başarısız."
                };

                await _publishEndpoint.Publish(paymentFailedEvent);
                await Console.Out.WriteLineAsync("Ödeme başarısız.");
            }
        }
    }
}
