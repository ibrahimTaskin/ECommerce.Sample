namespace Order.API.ViewModels
{
    public class CreateOrderVm
    {
        public Guid BuyerId { get; set; }
        public List<CreateOrderItemVm> CreateOrderItems { get; set; }
      
    }

    public class CreateOrderItemVm
    {
        public Guid ProductId { get; set; }
        public int Price { get; set; }
        public int Count { get; set; }
    }
}
