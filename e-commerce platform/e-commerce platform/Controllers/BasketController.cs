using Microsoft.AspNetCore.Mvc;
using e_commerce_platform.Extensions;
using e_commerce_platform.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class BasketController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public BasketController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
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
            return Json(new { success = false, message = "Product not found or is deleted." });
        }

        var basket = HttpContext.Session.Get<List<BasketItem>>("Basket") ?? new List<BasketItem>();
        var existingItem = basket.Find(item => item.ProductId == productId);
        if (existingItem != null)
        {
            if (existingItem.Quantity < product.Quantity)
            {
                existingItem.Quantity++;
            }
        }
        else
        {
            basket.Add(new BasketItem
            {
                ProductId = product.ProductID,
                ProductName = product.ProductName,
                Quantity = 1,
                Price = product.Price,
                Discount = product.Discount,
            });
        }

        HttpContext.Session.Set("Basket", basket);

        return Json(new { success = true, message = "Item added to basket." });
    }


    [HttpPost]
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
    [HttpPost]
    public IActionResult ClearBasket()
    {
        HttpContext.Session.Remove("Basket");
        return RedirectToAction("Index");
    }


    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var addresses = await _context.Address
                                      .Where(a => a.UserID == userId && !a.IsDeleted)
                                      .ToListAsync() ?? new List<Address>();

        var basket = HttpContext.Session.Get<List<BasketItem>>("Basket");

            var model = new CheckoutViewModel
        {
            Addresses = addresses,
            BasketItems = basket
        };

        return View(model);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(CheckoutViewModel model)
    {
        if (ModelState.IsValid)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var order = new Order
            {
                UserID = userId,
                AddressID = model.SelectedAddressID,
                TotalAmount = model.BasketItems.Sum(item => item.TotalPrice),
                
                OrderDetails = model.BasketItems.Select(item => new OrderDetail
                {
                    ProductID = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.PriceAfterDiscount,
                    Total = item.TotalPrice
                }).ToList()
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var item in model.BasketItems)
            {
                var product = await _context.Product.SingleOrDefaultAsync(p => p.ProductID == item.ProductId);
                if (product != null)
                {
                    product.Quantity -= item.Quantity;
                    if (product.Quantity < 0)
                    {
                        product.Quantity = 0; 
                    }
                }
            }

            await _context.SaveChangesAsync();
            HttpContext.Session.Remove("Basket");

            return RedirectToAction("Confirmation", new { orderId = order.OrderID });
        }

        var user = await _userManager.GetUserAsync(User);
        model.Addresses = await _context.Address.Where(a => a.UserID == user.Id && !a.IsDeleted).ToListAsync();

        return View(model);
    }


    public async Task<IActionResult> Confirmation(int orderId)
    {
        var order = await _context.Orders
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Product)
            .FirstOrDefaultAsync(o => o.OrderID == orderId);

        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }
    [HttpPost]
    public IActionResult IncreaseQuantity(int productId)
    {
        var basket = HttpContext.Session.Get<List<BasketItem>>("Basket") ?? new List<BasketItem>();

        var basketItem = basket.Find(item => item.ProductId == productId);

        if (basketItem != null)
        {
            var product = _context.Product.SingleOrDefault(p => p.ProductID == productId);
            if (product != null && basketItem.Quantity < product.Quantity)
            {
                basketItem.Quantity++;
                HttpContext.Session.Set("Basket", basket);
                var newTotal = basketItem.Quantity * basketItem.PriceAfterDiscount;
                return Json(new { success = true, newQuantity = basketItem.Quantity, newTotal = newTotal.ToString("C") });
            }
            return Json(new { success = false, message = "Insufficient stock available" });
        }
        return Json(new { success = false, message = "Product not found" });
    }


    [HttpPost]
    public IActionResult DecreaseQuantity(int productId)
    {
        var basket = HttpContext.Session.Get<List<BasketItem>>("Basket") ?? new List<BasketItem>();

        var basketItem = basket.Find(item => item.ProductId == productId);
        if (basketItem != null && basketItem.Quantity > 1)
        {
            basketItem.Quantity--;
            HttpContext.Session.Set("Basket", basket);
            var newTotal = basketItem.Quantity * basketItem.PriceAfterDiscount;
            return Json(new { success = true, newQuantity = basketItem.Quantity, newTotal = newTotal.ToString("C") });
        }
        return Json(new { success = false, message = "Quantity cannot be less than 1" });
    }


}

