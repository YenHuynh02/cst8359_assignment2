using System.ComponentModel.DataAnnotations;

namespace Lab5.Models
{
    public class Deal
    {

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Title of deal")]
        public string DealTitle { get; set; }

        [Display(Name = "Image URL")]
        [Required]
        [StringLength(50)]
        public string ImageURL { get; set; }



        public FoodDeliveryService FoodDeliveryService { get; set; }
        public string ServiceId { get; set; }

    }
}
