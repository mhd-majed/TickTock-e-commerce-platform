using e_commerce_platform.Models;
using e_commerce_platform.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace e_commerce_platform.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

		public IActionResult Index()
		{
			var latestProducts = _context.Product
										 .OrderByDescending(p => p.ProductID)
										 .Take(4)
										 .ToList();

			var approvedTestimonials = _context.Testimonial
											   .Where(t => t.Approved && !t.IsDeleted)
											   .Include(t => t.User) 
											   .OrderByDescending(t => t.CreatedAt)
											   .Take(3)
											   .ToList();

			var viewModel = new HomeViewModel
			{
				LatestProducts = latestProducts,
				ApprovedTestimonials = approvedTestimonials
			};

			return View(viewModel);
		}

		public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
