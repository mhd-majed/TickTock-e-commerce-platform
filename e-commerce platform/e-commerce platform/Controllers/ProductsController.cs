using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using e_commerce_platform.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;

namespace e_commerce_platform.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Products
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Product.Include(p => p.Category);
            return View(await applicationDbContext.ToListAsync());
        }
        

        public async Task<IActionResult> DisplayProducts()
        {
            var applicationDbContext = _context.Product.Include(p => p.Category);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = await _context.Product
                .Include(p => p.Category)
                .Include(p => p.AdditionalImages) 
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
        // GET: Products/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["CategoryID"] = new SelectList(_context.Category, "CategoryID", "CategoryName");
            return View();
        }



        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("ProductID,ProductName,CategoryID,Discount,Price,Quantity,ProductImage,Description,IsDeleted")] Product product, IFormFile ProductImage)
        {
            if (ProductImage != null && ProductImage.Length > 0)
            { 
                var fileName = Path.GetFileNameWithoutExtension(ProductImage.FileName);
                var extension = Path.GetExtension(ProductImage.FileName);
                fileName = $"{fileName}_{Guid.NewGuid()}{extension}";

                var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "product_images");
                var filePath = Path.Combine(uploadPath, fileName);

                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ProductImage.CopyToAsync(fileStream);
                }

                product.ProductImage = $"/uploads/product_images/{fileName}";

            }

            _context.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("UploadImages", "ProductImages", new { id = product.ProductID });

        }




        // GET: Products/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryID"] = new SelectList(_context.Category, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("ProductID,ProductName,CategoryID,Discount,Price,Quantity,ProductImage,Description,IsDeleted")] Product product, IFormFile ProductImage)
        {
            if (id != product.ProductID)
            {
                return NotFound();
            }
            try
            {
                if (ProductImage != null && ProductImage.Length > 0)
                {
                    // Generate a unique file name
                    var fileName = Path.GetFileNameWithoutExtension(ProductImage.FileName);
                    var extension = Path.GetExtension(ProductImage.FileName);
                    var uniqueFileName = $"{fileName}_{Guid.NewGuid()}{extension}";

                    // Define the path to save the image
                    var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "product_images");
                    var filePath = Path.Combine(uploadPath, uniqueFileName);

                    // Save the image to the server
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ProductImage.CopyToAsync(fileStream);
                    }

                    // Update the category image path
                    product.ProductImage = $"/uploads/product_images/{uniqueFileName}";
                }
                _context.Update(product);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(product.ProductID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
            
        }

        // GET: Products/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);
            if (product != null)
            {
                product.IsDeleted = true;
                _context.Product.Update(product);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.ProductID == id);
        }
    }
}
