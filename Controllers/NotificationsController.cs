using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TravelSphereMVC.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace TravelSphereMVC.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotificationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string? GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

        // ── All notifications for current traveller ────────────────────
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return View(notifications);
        }

        // ── Mark single notification as read ──────────────────────────
        [HttpPost]
        public async Task<IActionResult> MarkRead(string id)
        {
            var userId = GetUserId();
            var note   = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (note != null)
            {
                note.IsRead = true;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        // ── Mark all as read ──────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> MarkAllRead()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var unread = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var n in unread) n.IsRead = true;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
