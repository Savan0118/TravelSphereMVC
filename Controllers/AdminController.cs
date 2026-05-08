using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TravelSphereMVC.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace TravelSphereMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string? GetAdminId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

        // ── Notification helper ────────────────────────────────────────
        private async Task NotifyUserAsync(string userId, string title, string message)
        {
            _context.Notifications.Add(new Notification
            {
                Id        = Guid.NewGuid().ToString(),
                UserId    = userId,
                Title     = title,
                Message   = message,
                IsRead    = false,
                CreatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
        }

        // ── Dashboard ──────────────────────────────────────────────────
        public async Task<IActionResult> Index()
        {
            var adminId = GetAdminId();
            if (string.IsNullOrEmpty(adminId))
                return RedirectToAction("Login", "Account");

            ViewBag.TotalPackages = await _context.Packages
                .Where(p => p.AdminId == adminId)
                .CountAsync();

            ViewBag.TotalDestinations = await _context.Packages
                .Where(p => p.AdminId == adminId)
                .Select(p => p.Location)
                .Distinct()
                .CountAsync();

            ViewBag.TotalRevenue = await _context.Bookings
                .Where(b => b.Package!.AdminId == adminId && b.Status == "Confirmed")
                .SumAsync(b => (decimal?)b.TotalAmount) ?? 0m;

            ViewBag.RecentBookings = await _context.Bookings
                .Include(b => b.Package)
                .Include(b => b.User)
                .Include(b => b.Travellers)
                .Where(b => b.Package!.AdminId == adminId)
                .OrderByDescending(b => b.Timestamp)
                .Take(10)
                .ToListAsync();

            return View();
        }

        // ── Packages ───────────────────────────────────────────────────
        public async Task<IActionResult> Packages()
        {
            var adminId = GetAdminId();
            if (string.IsNullOrEmpty(adminId))
                return RedirectToAction("Login", "Account");

            var packages = await _context.Packages
                .Where(p => p.AdminId == adminId)
                .ToListAsync();

            return View(packages);
        }

        [HttpGet]
        public IActionResult AddPackage() => View();

        [HttpPost]
        public async Task<IActionResult> AddPackage(Package model)
        {
            var adminId = GetAdminId();
            if (string.IsNullOrEmpty(adminId))
                return RedirectToAction("Login", "Account");

            model.Id      = Guid.NewGuid().ToString();
            model.AdminId = adminId;

            // Remove all optional / auto-set fields from validation
            foreach (var key in new[]
            {
                "Id", "AdminId", "Admin", "Bookings", "WishlistItems",
                "Country", "State", "DestinationType",
                "ShortDescription", "FullDescription",
                "PickupPoint", "DropPoint", "BestSeason",
                "TravelType", "DifficultyLevel",
                "HotelName", "HotelRating", "RoomType", "MealsIncluded",
                "DiscountPercentage", "GalleryImages", "SpecialAttractions"
            })
            {
                ModelState.Remove(key);
            }

            _context.Packages.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("Packages");
        }

        // ── Bookings ───────────────────────────────────────────────────
        public async Task<IActionResult> Bookings(string? search)
        {
            var adminId = GetAdminId();
            if (string.IsNullOrEmpty(adminId))
                return RedirectToAction("Login", "Account");

            IQueryable<Booking> bookings = _context.Bookings
                .Include(b => b.Package)
                .Include(b => b.User)
                .Include(b => b.Travellers)
                .Where(b => b.Package!.AdminId == adminId);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                bookings = bookings.Where(b =>
                    (b.Package != null && b.Package.Name.ToLower().Contains(s)) ||
                    (b.User    != null && b.User.Name.ToLower().Contains(s))    ||
                    b.Status.ToLower().Contains(s));
            }

            return View(await bookings.OrderByDescending(b => b.Timestamp).ToListAsync());
        }

        // ── Update booking status + notify traveller ───────────────────
        [HttpPost]
        public async Task<IActionResult> UpdateBookingStatus(string id, string status)
        {
            var adminId = GetAdminId();
            var booking = await _context.Bookings
                .Include(b => b.Package)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking != null && booking.Package?.AdminId == adminId)
            {
                var prevStatus = booking.Status;
                booking.Status = status;
                await _context.SaveChangesAsync();

                if (status == "Confirmed" && prevStatus != "Confirmed")
                {
                    await NotifyUserAsync(
                        booking.UserId,
                        "Booking Confirmed! 🎉",
                        $"Great news! Your booking for \"{booking.Package?.Name}\" has been confirmed. Get ready to travel!"
                    );
                }
                else if (status == "Cancelled" && prevStatus != "Cancelled")
                {
                    await NotifyUserAsync(
                        booking.UserId,
                        "Booking Cancelled",
                        $"We're sorry — your booking for \"{booking.Package?.Name}\" has been cancelled. Please contact support for assistance."
                    );
                }
            }
            return RedirectToAction("Bookings");
        }

        [HttpPost]
        public async Task<IActionResult> ApproveBooking(string id)
            => await UpdateBookingStatus(id, "Confirmed");

        // ── Profile ────────────────────────────────────────────────────
        public async Task<IActionResult> Profile()
        {
            var adminId = GetAdminId();
            if (string.IsNullOrEmpty(adminId))
                return RedirectToAction("Login", "Account");

            var admin = await _context.Users.FindAsync(adminId);

            ViewBag.PackageCount = await _context.Packages
                .Where(p => p.AdminId == adminId)
                .CountAsync();

            ViewBag.BookingCount = await _context.Bookings
                .Where(b => b.Package!.AdminId == adminId)
                .CountAsync();

            return View(admin);
        }

        public IActionResult About() => View();

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var adminId = GetAdminId();
            if (string.IsNullOrEmpty(adminId))
                return RedirectToAction("Login", "Account");

            var admin = await _context.Users.FindAsync(adminId);
            return View(admin);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(string Name, string Email, string ProfileImageUrl)
        {
            var adminId = GetAdminId();
            if (string.IsNullOrEmpty(adminId))
                return RedirectToAction("Login", "Account");

            var admin = await _context.Users.FindAsync(adminId);
            if (admin != null)
            {
                admin.Name            = Name;
                admin.Email           = Email;
                admin.ProfileImageUrl = ProfileImageUrl;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Profile");
        }
    }
}
