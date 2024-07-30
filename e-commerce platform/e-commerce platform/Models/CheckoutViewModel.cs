using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace e_commerce_platform.Models
{
    public class CheckoutViewModel
    {
        public List<Address>? Addresses { get; set; } = new List<Address>();
        public List<BasketItem>? BasketItems { get; set; }

        [Required]
        public int SelectedAddressID { get; set; }

        [Required]
        [StringLength(16)]
        [RegularExpression(@"^\d+$", ErrorMessage = "Card number must be numeric.")]
        public string CardNumber { get; set; }

        [Required]
        [StringLength(5)]
        [CustomExpiryDateValidation]
        public string ExpiryDate { get; set; }

        [Required]
        [StringLength(3)]
        [RegularExpression(@"^\d+$", ErrorMessage = "CVV must be numeric.")]
        public string CVV { get; set; }



        public class CustomExpiryDateValidationAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var expiryDateStr = (string)value;
                if (string.IsNullOrWhiteSpace(expiryDateStr) || !Regex.IsMatch(expiryDateStr, @"^\d{2}/\d{2}$"))
                {
                    return new ValidationResult("Expiry Date must be in the format MM/YY.");
                }

                if (DateTime.TryParseExact(expiryDateStr, "MM/yy", null, System.Globalization.DateTimeStyles.None, out DateTime expiryDate))
                {
                    if (expiryDate < DateTime.Now)
                    {
                        return new ValidationResult("Expiry Date cannot be in the past.");
                    }
                }
                else
                {
                    return new ValidationResult("Expiry Date format is invalid.");
                }

                return ValidationResult.Success;
            }
        }

    }
}
