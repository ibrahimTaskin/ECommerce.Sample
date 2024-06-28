using MassTransit;
using MongoDB.Driver;
using Shared.Events;
using Shared.Messages;
using Shared.Settings;
using Stock.API.Services;

namespace Stock.API.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
    {
        IMongoCollection<Stock.API.Models.Entities.Stock> _stockCollection;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly IPublishEndpoint _publishEndpoint;
        public OrderCreatedEventConsumer(IMongoDbService mongoDbService, ISendEndpointProvider sendEndpointProvider, IPublishEndpoint publishEndpoint)
        {
            _stockCollection = mongoDbService.GetCollection<Stock.API.Models.Entities.Stock>();
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpoint = publishEndpoint;
        }
        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            List<bool> stockResult = new();
            foreach (OrderItemMessage orderItem in context.Message.OrderItems)
            {
                stockResult.Add((await _stockCollection.FindAsync(s => s.ProductId == orderItem.ProductId && s.Count >= orderItem.Count)).Any());
            }
            if (stockResult.TrueForAll(sr => sr.Equals(true))) // bütün stoklar ok
            {
                foreach (OrderItemMessage orderItem in context.Message.OrderItems)
                {
                    var stock = (await _stockCollection.FindAsync(s => s.ProductId == orderItem.ProductId)).FirstOrDefault();
                    stock.Count -= orderItem.Count;
                    await _stockCollection.FindOneAndReplaceAsync(s => s.ProductId == orderItem.ProductId, stock);
                }
                // Payment event tetiklenir
                StockReservedEvent stockReservedEvent = new StockReservedEvent()
                {
                    OrderId = context.Message.OrderId,
                    BuyerId = context.Message.BuyerId,
                    TotalPrice = context.Message.TotalPrice
                };

                // Tek bir yer dinlediği için send ile ilgili consumer'a gonderdik.
                ISendEndpoint sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMqSettings.OrderStokReservedEventQueue}"));
                await sendEndpoint.Send(stockReservedEvent);

                await Console.Out.WriteLineAsync("Stok işlemleri başarılı.");
            }
            else // Stok yetersiz. İlgili OrderId'ye git ve başarısız olarak işaretle
            {
                StockNotReservedEvent stockNotReservedEvent = new()
                {
                    OrderId = context.Message.OrderId,
                    BuyerId = context.Message.BuyerId,
                    Message = "Stok yetersiz."
                };
                // Birden çok yer dinliyor olabilir. O yüzden publish ettik.
                await _publishEndpoint.Publish(stockNotReservedEvent);

                await Console.Out.WriteLineAsync("Stok işlemleri başarısız.");
            }
        }
    }
}
