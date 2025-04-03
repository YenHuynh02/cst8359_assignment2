namespace Lab5.Models.ViewModels
{
    public class DealsPostsViewModel
    {

        public FoodDeliveryService FoodDeliveryService { get; set; }
        public IEnumerable<Deal> Deals { get; set; }

    }
}
