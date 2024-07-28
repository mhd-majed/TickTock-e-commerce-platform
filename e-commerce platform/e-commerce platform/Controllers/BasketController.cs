using Microsoft.AspNetCore.Mvc;
using e_commerce_platform.Extensions;
using e_commerce_platform.Models;
using System.Collections.Generic;

public class BasketController : Controller
{
    private readonly ApplicationDbContext _context;

    public BasketController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var basket = HttpContext.Session.Get<List<BasketItem>>("Basket") ?? new List<BasketItem>();
        return View(basket);
    }

    public IActionResult AddToBasket(int productId)
    {
        var product = _context.Product.Find(productId);
        if (product == null || product.IsDeleted)
        {
            return NotFound();
        }

        var basket = HttpContext.Session.Get<List<BasketItem>>("Basket") ?? new List<BasketItem>();

        var existingItem = basket.Find(item => item.ProductId == productId);
        if (existingItem != null)
        {
            existingItem.Quantity++;
        }
        else
        {
            basket.Add(new BasketItem
            {
                ProductId = product.ProductID,
                ProductName = product.ProductName,
                Quantity = 1,
                Price = product.Price,
                Discount = product.Discount
            });
        }

        HttpContext.Session.Set("Basket", basket);

        return RedirectToAction("Index");
    }

    public IActionResult RemoveFromBasket(int productId)
    {
        var basket = HttpContext.Session.Get<List<BasketItem>>("Basket") ?? new List<BasketItem>();

        var basketItem = basket.Find(item => item.ProductId == productId);

        if (basketItem != null)
        {
            basket.Remove(basketItem);
            HttpContext.Session.Set("Basket", basket);
        }

        return RedirectToAction("Index");
    }
}
