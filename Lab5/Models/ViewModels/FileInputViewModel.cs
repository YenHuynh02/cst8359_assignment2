namespace Lab5.Models.ViewModels
{
    public class FileInputViewModel
    {

        public string FoodDeliveryServiceId { get; set; }

        public string FoodDeliveryServiceTitle { get; set; }
        public IFormFile File { get; set; }

    }
}
