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
        public OrderCreatedEventConsumer(IMongoDbService mongoDbService, ISendEndpointProvider sendEndpointProvider)
        {
            _stockCollection = mongoDbService.GetCollection<Stock.API.Models.Entities.Stock>();
            _sendEndpointProvider = sendEndpointProvider;
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

                ISendEndpoint sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue: {RabbitMqSettings.PaymentStokReservedEventQueue}"));
                await sendEndpoint.Send(stockReservedEvent);
            }
            else // Stok yetersiz 
            {

            }
            Console.WriteLine(context.Message.OrderId + " " + context.Message.BuyerId);
        }
    }
}
