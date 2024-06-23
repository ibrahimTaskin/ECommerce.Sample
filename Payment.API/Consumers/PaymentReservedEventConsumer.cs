﻿using MassTransit;
using Shared.Events;

namespace Payment.API.Consumers
{
    public class PaymentReservedEventConsumer : IConsumer<StockReservedEvent>
    {

        readonly IPublishEndpoint _publishEndpoint;
        public PaymentReservedEventConsumer(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }
        public async Task Consume(ConsumeContext<StockReservedEvent> context)
        {
            if (true) // Ödeme başarılı
            {
                PaymentCompletedEvent paymentCompletedEvent = new()
                {
                    OrderId = context.Message.OrderId
                };

                await _publishEndpoint.Publish(paymentCompletedEvent);
            }
            else
            {
                // Ödemede sıkıntı var.
            }
        }
    }
}
