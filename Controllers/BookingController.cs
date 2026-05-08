using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TravelSphereMVC.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace TravelSphereMVC.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string? GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

        // ── Notification helper ────────────────────────────────────────
        private async Task CreateNotificationAsync(string userId, string title, string message)
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

        // ── Create booking (GET) ───────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Create(string packageId)
        {
            var package = await _context.Packages.FindAsync(packageId);
            if (package == null) return NotFound("Package not found.");

            ViewBag.Package = package;
            return View();
        }

        // ── Create booking (POST) ──────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> Create(
            Booking           model,
            List<string>      travellerNames,
            List<int>         travellerAges,
            List<string>      travellerTypes,
            string            packageId,
            decimal           totalAmount)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            model.UserId      = userId;
            model.PackageId   = packageId;
            model.Id          = Guid.NewGuid().ToString();
            model.Timestamp   = DateTime.UtcNow;
            model.Status      = "Pending";
            model.TotalAmount = totalAmount;

            var travellers = new List<Traveller>();
            if (travellerNames != null)
            {
                for (int i = 0; i < travellerNames.Count; i++)
                {
                    travellers.Add(new Traveller
                    {
                        Id            = Guid.NewGuid().ToString(),
                        Name          = travellerNames[i],
                        Age           = travellerAges  != null && i < travellerAges.Count  ? travellerAges[i]  : 0,
                        TravellerType = travellerTypes != null && i < travellerTypes.Count ? travellerTypes[i] : "Adult",
                        BookingId     = model.Id
                    });
                }
            }
            model.Travellers = travellers;

            _context.Bookings.Add(model);
            await _context.SaveChangesAsync();

            // Notification
            var pkg = await _context.Packages.FindAsync(packageId);
            await CreateNotificationAsync(
                userId,
                "Booking Submitted 📋",
                $"Your booking request for \"{pkg?.Name ?? packageId}\" has been submitted. Our team will confirm shortly."
            );

            return RedirectToAction("MyJourneys");
        }

        // ── My Journeys ────────────────────────────────────────────────
        public async Task<IActionResult> MyJourneys()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var bookings = await _context.Bookings
                .Include(b => b.Package)
                .Include(b => b.Travellers)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.Timestamp)
                .ToListAsync();

            return View(bookings);
        }

        // ── Trip Details (ownership verified) ─────────────────────────
        public async Task<IActionResult> TripDetails(string id)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var booking = await _context.Bookings
                .Include(b => b.Package)
                    .ThenInclude(p => p!.Reviews)
                        .ThenInclude(r => r.User)
                .Include(b => b.Travellers)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)       return NotFound();
            if (booking.UserId != userId) return Forbid();

            // Can review?
            ViewBag.CanReview = booking.Status == "Confirmed"
                && !await _context.Reviews.AnyAsync(r => r.UserId == userId && r.PackageId == booking.PackageId);

            return View(booking);
        }
    }
}
