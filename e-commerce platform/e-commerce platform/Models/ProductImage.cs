using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace e_commerce_platform.Models
{
    public class ProductImage
    {
        [Key]
        public int ImageID { get; set; }

        [ForeignKey("Product")]
        public int ProductID { get; set; }

        public Product Product { get; set; }

        [Required]
        [StringLength(255)]
        public string ImageUrl { get; set; }
    }
}
