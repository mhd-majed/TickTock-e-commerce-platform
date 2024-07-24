using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace e_commerce_platform.Models
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductName { get; set; }

        [ForeignKey("Category")]
        public int? CategoryID { get; set; }

        public Category Category { get; set; }
        
        [Column(TypeName = "decimal(5, 2)")]
        public decimal Discount { get; set; } = 0;

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        [Required]
        public int Quantity { get; set; }

        [StringLength(255)]
        public string ProductImage { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }
        public bool IsDeleted { get; set; } = false;

        [NotMapped]
        public decimal PriceAfterDiscount => Price - (Price * (Discount / 100));
    }
}
