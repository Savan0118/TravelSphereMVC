using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TravelSphereMVC.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace TravelSphereMVC.Controllers
{
    [Authorize]
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReviewController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string? GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

        // ── Submit Review (POST) ───────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> Submit(string packageId, int rating, string comment)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            // Ensure traveller has a CONFIRMED booking for this package
            var hasBooking = await _context.Bookings
                .AnyAsync(b => b.UserId == userId
                            && b.PackageId == packageId
                            && b.Status == "Confirmed");

            if (!hasBooking)
            {
                TempData["ReviewError"] = "You can only review packages you have confirmed bookings for.";
                return RedirectToAction("Description", "Home", new { id = packageId });
            }

            // Prevent duplicate reviews
            var alreadyReviewed = await _context.Reviews
                .AnyAsync(r => r.UserId == userId && r.PackageId == packageId);

            if (alreadyReviewed)
            {
                TempData["ReviewError"] = "You have already submitted a review for this package.";
                return RedirectToAction("Description", "Home", new { id = packageId });
            }

            _context.Reviews.Add(new Review
            {
                Id        = Guid.NewGuid().ToString(),
                UserId    = userId,
                PackageId = packageId,
                Rating    = Math.Clamp(rating, 1, 5),
                Comment   = comment,
                CreatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            TempData["ReviewSuccess"] = "Thank you! Your review has been submitted.";
            return RedirectToAction("Description", "Home", new { id = packageId });
        }
    }
}
