namespace Shared.Settings
{
    public static class RabbitMqSettings
    {
        public const string OrderCreatedEventQueue = "order-created-event-queue";
        public const string OrderStokReservedEventQueue = "order-stock-reserved-event-queue";
        public const string OrderStokNotReservedEventQueue = "order-stock-not-reserved-event-queue";
        public const string PaymentCompletedEventQueue = "payment-completed-event-queue";
        public const string PaymentFailedEventQueue = "payment-failed-event-queue";
    }
}
