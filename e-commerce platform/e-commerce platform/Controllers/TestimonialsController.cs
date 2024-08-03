using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using e_commerce_platform.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace e_commerce_platform.Controllers
{
    public class TestimonialsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TestimonialsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Testimonials
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string searchQuery)
        {
            var applicationDbContext = _context.Testimonial.Include(t => t.User).AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                searchQuery = searchQuery.ToLower();
                applicationDbContext = applicationDbContext.Where(t =>
                    t.User.Id.ToLower().Contains(searchQuery) ||
                    t.Comment.ToLower().Contains(searchQuery) ||
                    t.Approved.ToString().ToLower().Contains(searchQuery) // Adjust based on how you handle "Approved"
                );
            }

            ViewData["CurrentFilter"] = searchQuery;

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Testimonials/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testimonial = await _context.Testimonial
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.TestimonialID == id);
            if (testimonial == null)
            {
                return NotFound();
            }

            return View(testimonial);
        }

        // GET: Testimonials/Create
        public IActionResult Create()
        {
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Testimonials/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TestimonialID,Rating,Comment")] Testimonial testimonial)
        {
            
			var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (UserId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            // Check if the user already has a testimonial
            var existingTestimonial = await _context.Testimonial
                .FirstOrDefaultAsync(t => t.UserID == UserId && !t.IsDeleted);

            if (existingTestimonial != null)
            {
                // User already has a testimonial
                ViewBag.AlreadySubmitted = true;
                return View(testimonial);
            }

            testimonial.UserID = UserId;
			_context.Add(testimonial);
			await _context.SaveChangesAsync();
			return RedirectToAction("MyTestimonials");
			
			
		}
        public async Task<IActionResult> MyTestimonials()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var testimonial = await _context.Testimonial
                .Where(t => t.UserID == user.Id && !t.IsDeleted)
                .FirstOrDefaultAsync();

            if (testimonial == null)
            {
                // Log or debug information here
                return View("Create");
            }

            return View(testimonial);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testimonial = await _context.Testimonial.FindAsync(id);
            if (testimonial == null)
            {
                return NotFound();
            }
            return View(testimonial);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TestimonialID,Rating,Comment")] Testimonial testimonial)
        {
            if (id != testimonial.TestimonialID)
            {
                return NotFound();
            }

            try
            {
                var existingTestimonial = await _context.Testimonial.FindAsync(id);
                if (existingTestimonial == null)
                {
                    return NotFound();
                }

                existingTestimonial.Rating = testimonial.Rating;
                existingTestimonial.Comment = testimonial.Comment;
                existingTestimonial.UpdatedAt = DateTime.Now;

                _context.Update(existingTestimonial);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TestimonialExists(testimonial.TestimonialID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("MyTestimonials");
        }



        // GET: Testimonials/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testimonial = await _context.Testimonial
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.TestimonialID == id);
            if (testimonial == null)
            {
                return NotFound();
            }

            return View(testimonial);
        }

        // POST: Testimonials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var testimonial = await _context.Testimonial.FindAsync(id);
            if (testimonial != null)
            {
                testimonial.IsDeleted = true;
                _context.Testimonial.Update(testimonial);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Testimonials/Approve/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testimonial = await _context.Testimonial
                .FirstOrDefaultAsync(m => m.TestimonialID == id);
            if (testimonial == null)
            {
                return NotFound();
            }

            return View(testimonial);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveConfirmed( int TestimonialID)
        {
            var testimonial = await _context.Testimonial.FindAsync(TestimonialID);

            if (testimonial != null)
            {
                testimonial.Approved = true;
                _context.Update(testimonial);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); 
            }

            return NotFound();
        }



        private bool TestimonialExists(int id)
        {
            return _context.Testimonial.Any(e => e.TestimonialID == id);
        }
    }
}
