using e_commerce_platform.Models;
using Microsoft.AspNetCore.Http;
using e_commerce_platform.Extensions;



namespace e_commerce_platform.Services
{
    public class BasketService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BasketService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private List<BasketItem> Basket
        {
            get
            {
                var session = _httpContextAccessor.HttpContext.Session;
                var basket = session.Get<List<BasketItem>>("Basket") ?? new List<BasketItem>();
                return basket;
            }
            set
            {
                var session = _httpContextAccessor.HttpContext.Session;
                session.Set("Basket", value);
            }
        }

        public void AddToBasket(BasketItem item)
        {
            var basket = Basket;
            var existingItem = basket.FirstOrDefault(i => i.ProductId == item.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                basket.Add(item);
            }

            Basket = basket;
        }

        public void RemoveFromBasket(int productId)
        {
            var basket = Basket;
            var item = basket.FirstOrDefault(i => i.ProductId == productId);

            if (item != null)
            {
                basket.Remove(item);
            }

            Basket = basket;
        }

        public List<BasketItem> GetBasket()
        {
            return Basket;
        }

        public void ClearBasket()
        {
            Basket = new List<BasketItem>();
        }
    }

}
