using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TravelSphereMVC.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace TravelSphereMVC.Controllers
{
    [Authorize]
    public class WishlistController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WishlistController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string? GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

        // ── Wishlist index ─────────────────────────────────────────────
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var items = await _context.WishlistItems
                .Include(w => w.Package)
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.AddedAt)
                .ToListAsync();

            return View(items);
        }

        // ── Add to wishlist ────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> Add(string packageId, string? returnUrl)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            // Prevent duplicate entries
            var exists = await _context.WishlistItems
                .AnyAsync(w => w.UserId == userId && w.PackageId == packageId);

            if (!exists)
            {
                _context.WishlistItems.Add(new WishlistItem
                {
                    Id        = Guid.NewGuid().ToString(),
                    UserId    = userId,
                    PackageId = packageId,
                    AddedAt   = DateTime.UtcNow
                });
                await _context.SaveChangesAsync();
                TempData["WishlistMessage"] = "Added to wishlist!";
            }
            else
            {
                TempData["WishlistMessage"] = "Already in your wishlist.";
            }

            return Redirect(returnUrl ?? Url.Action("Index", "Wishlist")!);
        }

        // ── Remove from wishlist ───────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> Remove(string packageId, string? returnUrl)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var item = await _context.WishlistItems
                .FirstOrDefaultAsync(w => w.UserId == userId && w.PackageId == packageId);

            if (item != null)
            {
                _context.WishlistItems.Remove(item);
                await _context.SaveChangesAsync();
                TempData["WishlistMessage"] = "Removed from wishlist.";
            }

            return Redirect(returnUrl ?? Url.Action("Index", "Wishlist")!);
        }
    }
}
