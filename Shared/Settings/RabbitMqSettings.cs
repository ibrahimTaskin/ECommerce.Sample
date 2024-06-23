namespace Shared.Settings
{
    public static class RabbitMqSettings
    {
        public const string StockOrderCreatedEventQueue = "stock-order-created-event-queue";
        public const string PaymentStokReservedEventQueue = "payment-stock-reserved-event-queue";
        public const string PaymentCompletedEventQueue = "payment-completed-event-queue";
    }
}
