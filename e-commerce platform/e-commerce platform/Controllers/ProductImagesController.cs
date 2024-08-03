using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using e_commerce_platform.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace e_commerce_platform.Controllers
{
    public class ProductImagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductImagesController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: ProductImages
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string searchQuery)
        {
            ViewData["CurrentFilter"] = searchQuery;

            var productImages = _context.ProductImage.Include(p => p.Product).AsQueryable();

            if (!String.IsNullOrEmpty(searchQuery))
            {
                searchQuery = searchQuery.ToLower();
                productImages = productImages.Where(p =>
                    p.Product.ProductName.ToLower().Contains(searchQuery) ||
                    p.ImageUrl.ToLower().Contains(searchQuery)
                );
            }

            return View(await productImages.ToListAsync());
        }


        [Authorize(Roles = "Admin")]
        // GET: ProductImages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productImage = await _context.ProductImage
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.ImageID == id);
            if (productImage == null)
            {
                return NotFound();
            }

            return View(productImage);
        }

        // GET: ProductImages/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["ProductID"] = new SelectList(_context.Product, "ProductID", "ProductName");
            return View();
        }

        // POST: ProductImages/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(int productId, List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                ModelState.AddModelError("File", "Please upload a file.");
                ViewData["ProductID"] = new SelectList(_context.Product, "ProductID", "ProductName", productId);
                return View();
            }

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    // Generate unique file name and save to wwwroot/images
                    var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                    var extension = Path.GetExtension(file.FileName);
                    var newFileName = $"{fileName}_{DateTime.Now.ToString("yyyyMMddHHmmss")}{extension}";
                    var path = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "product_images", newFileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Save image record in the database
                    var productImage = new ProductImage
                    {
                        ProductID = productId,
                        ImageUrl = $"/uploads/product_images/{newFileName}"
                    };
                    _context.ProductImage.Add(productImage);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult UploadImages(int id)
        {
            ViewBag.ProductID = id;
            ViewBag.ProductName = _context.Product.Find(id).ProductName;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadImages(int ProductID, List<IFormFile> files)
        {
            Console.WriteLine("sfijisfjipddddddk");
            if (files == null || files.Count == 0)
            {
                ModelState.AddModelError("File", "Please upload at least one file.");
                return View();
            }

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                    var extension = Path.GetExtension(file.FileName);
                    var newFileName = $"{fileName}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                    var path = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "product_images", newFileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var productImage = new ProductImage
                    {
                        ProductID = ProductID,
                        ImageUrl = $"/uploads/product_images/{newFileName}"
                    };
                    _context.ProductImage.Add(productImage);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: ProductImages/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productImage = await _context.ProductImage.FindAsync(id);
            if (productImage == null)
            {
                return NotFound();
            }
            ViewData["ProductID"] = new SelectList(_context.Product, "ProductID", "ProductName", productImage.ProductID);
            return View(productImage);
        }

        // POST: ProductImages/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("ImageID,ProductID,ImageUrl")] ProductImage productImage)
        {
            if (id != productImage.ImageID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productImage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductImageExists(productImage.ImageID))
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
            ViewData["ProductID"] = new SelectList(_context.Product, "ProductID", "ProductName", productImage.ProductID);
            return View(productImage);
        }

        // GET: ProductImages/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productImage = await _context.ProductImage
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.ImageID == id);
            if (productImage == null)
            {
                return NotFound();
            }

            return View(productImage);
        }

        // POST: ProductImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productImage = await _context.ProductImage.FindAsync(id);
            if (productImage != null)
            {
                _context.ProductImage.Remove(productImage);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductImageExists(int id)
        {
            return _context.ProductImage.Any(e => e.ImageID == id);
        }
    }
}
