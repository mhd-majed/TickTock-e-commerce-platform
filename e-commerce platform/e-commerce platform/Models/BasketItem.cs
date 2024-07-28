namespace e_commerce_platform.Models
{
    public class BasketItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal PriceAfterDiscount => Price - (Price * (Discount / 100));
        public decimal TotalPrice => Quantity * PriceAfterDiscount;
    }
}
