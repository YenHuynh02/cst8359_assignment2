namespace Lab5.Models.ViewModels
{
    public class DealsViewModel
    {
        public IEnumerable<Customer> Customers { get; set; }
        public IEnumerable<FoodDeliveryService> FoodDeliveryServices { get; set; }
        public IEnumerable<Subscription> Subscriptions { get; set; }
    }
}
