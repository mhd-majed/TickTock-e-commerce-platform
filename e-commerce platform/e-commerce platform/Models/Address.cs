using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace e_commerce_platform.Models
{
    public class Address
    {
        [Key]
        public int AddressID { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }
        public User User { get; set; }  // Navigation property

        [StringLength(255)]
        public string Street { get; set; }

        [StringLength(100)]
        public string City { get; set; }

        [StringLength(100)]
        public string State { get; set; }

        [StringLength(20)]
        public string PostalCode { get; set; }

        [StringLength(100)]
        public string Country { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public ICollection<Order> Orders { get; set; }  // Navigation property for orders associated with this address
    }
}
