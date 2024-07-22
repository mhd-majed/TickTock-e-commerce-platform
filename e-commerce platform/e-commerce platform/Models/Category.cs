using System.ComponentModel.DataAnnotations;

namespace e_commerce_platform.Models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }

        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        [StringLength(255)]
        public string CategoryImage { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
