using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TravelSphereMVC.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace TravelSphereMVC.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    // ── Home — dynamic trending + featured packages ────────────────────
    public async Task<IActionResult> Index()
    {
        // Show only packages that have been published by any admin
        var trending = await _context.Packages
            .Where(p => p.AdminId != null)
            .OrderBy(p => p.Price)
            .Take(3)
            .ToListAsync();

        var featured = await _context.Packages
            .Where(p => p.AdminId != null)
            .OrderByDescending(p => p.Price)
            .FirstOrDefaultAsync();

        ViewBag.TrendingPackages = trending;
        ViewBag.FeaturedPackage  = featured;

        return View();
    }

    public IActionResult About()   => View();
    public IActionResult Weather() => View();

    // ── All packages — travellers browse everything ────────────────────
    public async Task<IActionResult> Packages(string? query, string? type, string? sort)
    {
        IQueryable<Package> packages = _context.Packages
            .Where(p => p.AdminId != null);  // only published packages

        if (!string.IsNullOrEmpty(query))
        {
            packages = packages.Where(p =>
                p.Name.Contains(query) ||
                p.Location.Contains(query) ||
                (p.Country != null && p.Country.Contains(query)));
        }

        if (!string.IsNullOrEmpty(type))
        {
            packages = packages.Where(p => p.DestinationType == type);
        }

        packages = sort switch
        {
            "price_asc"  => packages.OrderBy(p => p.Price),
            "price_desc" => packages.OrderByDescending(p => p.Price),
            "name"       => packages.OrderBy(p => p.Name),
            _            => packages.OrderByDescending(p => p.Id)
        };

        ViewBag.CurrentType  = type;
        ViewBag.CurrentSort  = sort;
        ViewBag.CurrentQuery = query;

        return View(await packages.ToListAsync());
    }

    // ── Package detail — includes reviews ─────────────────────────────
    public async Task<IActionResult> Description(string id)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();

        var package = await _context.Packages
            .Include(p => p.Reviews)
                .ThenInclude(r => r.User)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (package == null) return NotFound();

        // Check if current traveller can review (confirmed booking, not yet reviewed)
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        ViewBag.CanReview = !string.IsNullOrEmpty(userId)
            && await _context.Bookings.AnyAsync(b => b.UserId == userId && b.PackageId == id && b.Status == "Confirmed")
            && !await _context.Reviews.AnyAsync(r => r.UserId == userId && r.PackageId == id);

        // Average rating
        ViewBag.AverageRating = package.Reviews.Any()
            ? package.Reviews.Average(r => r.Rating)
            : 0.0;

        // Wishlist state
        var inWishlist = !string.IsNullOrEmpty(userId)
            && await _context.WishlistItems.AnyAsync(w => w.UserId == userId && w.PackageId == id);
        ViewBag.InWishlist = inWishlist;

        return View(package);
    }

    public IActionResult BudgetPlanner() => View();

    [HttpGet]
    [HttpPost]
    public IActionResult BudgetResult(string totalCalculated, string formState)
    {
        ViewBag.TotalCalculated = totalCalculated;
        ViewBag.FormState       = formState;
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
