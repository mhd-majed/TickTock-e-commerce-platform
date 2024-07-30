using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using e_commerce_platform.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace e_commerce_platform.Controllers
{
    public class AddressesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AddressesController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        public AddressesController(ApplicationDbContext context, ILogger<AddressesController> logger, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _logger.LogInformation("AddressesController instantiated with valid dependencies.");
        }

        // GET: Addresses
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Address.Include(a => a.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Addresses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var address = await _context.Address
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.AddressID == id);
            if (address == null)
            {
                return NotFound();
            }

            return View(address);
        }

        // GET: Addresses/Create
        public IActionResult Create()
        {
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Addresses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AddressID,Street,City,State,PostalCode,Country")] Address address)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            address.UserID = userId;

            var user = await _context.Users.FindAsync(userId);
            address.User = user;

            if (userId != null)
                {
                    _context.Add(address);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id", address.UserID);
            return View(address);
        }

        // GET: Addresses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            Console.WriteLine(id);
            if (id == null)
            {
                return NotFound();
            }

            var address = await _context.Address.FindAsync(id);
            if (address == null)
            {
                return NotFound();
            }
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id", address.UserID);
            return View(address);
        }

        // POST: Addresses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AddressID,Street,City,State,PostalCode,Country,")] Address address)
        {

            if (id != address.AddressID)
            {
                return NotFound();
            }
            try
            {
                var existingAddress = await _context.Address.FindAsync(address.AddressID);
                if (existingAddress == null)
                {
                    return NotFound();
                }
                existingAddress.Street = address.Street;
                existingAddress.City = address.City;
                existingAddress.State = address.State;
                existingAddress.PostalCode = address.PostalCode;
                existingAddress.Country = address.Country;
                existingAddress.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AddressExists(address.AddressID))
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

        // GET: Addresses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var address = await _context.Address
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.AddressID == id);
            if (address == null)
            {
                return NotFound();
            }

            return View();
        }

        // POST: Addresses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var address = await _context.Address.FindAsync(id);
            if (address != null)
            {
                address.IsDeleted = true;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> MyAddress()
        {
            Console.WriteLine("dskjhfkusdhfuhsdiufhsuidhfi7usd");
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var addresses = await _context.Address
                .Where(a => a.UserID == user.Id && !a.IsDeleted)
                .ToListAsync();

            return View(addresses);
        }
        private bool AddressExists(int id)
        {
            return _context.Address.Any(e => e.AddressID == id);
        }
    }
}
