using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab5.Models
{
    public class Deal
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Title of deal")]
        public string DealTitle { get; set; }

        [Display(Name = "Image URL")]
        [Required]
        [StringLength(50)]
        public string ImageURL { get; set; }


        [ForeignKey("FoodDeliveryServiceId")]
        public FoodDeliveryService FoodDeliveryService { get; set; }
        public string FoodDeliveryServiceId { get; set; }

    }
}
