using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using TravelSphereMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace TravelSphereMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ── Login ──────────────────────────────────────────────────────
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.Users.FirstOrDefaultAsync(
                u => u.Email == model.Email && u.PasswordHash == model.PasswordHash);

            if (user == null)
            {
                ViewBag.Error = "Invalid email or password.";
                return View(model);
            }

            if (!string.Equals(user.Role, model.Role, StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.Error = $"Unauthorized: You are registered as a {user.Role}. Please select the correct role tab.";
                return View(model);
            }

            await SignInUserAsync(user);

            return user.Role == "Admin"
                ? RedirectToAction("Index", "Admin")
                : RedirectToAction("Index", "Home");
        }

        // ── Google OAuth ───────────────────────────────────────────────
        [HttpPost]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse") };
            return Challenge(properties, Microsoft.AspNetCore.Authentication.Google.GoogleDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!result.Succeeded)
                return RedirectToAction("Login");

            var email = result.Principal?.FindFirst(ClaimTypes.Email)?.Value;
            var name  = result.Principal?.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                user = new User
                {
                    Id               = Guid.NewGuid().ToString(),
                    Email            = email,
                    Name             = name ?? "Google User",
                    Mobile           = "N/A",
                    PasswordHash     = "GoogleAuth_" + Guid.NewGuid().ToString("N")[..8],
                    Role             = "Traveller",
                    RegistrationDate = DateTime.UtcNow
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            await SignInUserAsync(user);
            return RedirectToAction("Index", "Home");
        }

        // ── Register ───────────────────────────────────────────────────
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(User model)
        {
            model.Id               = Guid.NewGuid().ToString();
            model.RegistrationDate = DateTime.UtcNow;

            // Remove fields that are set by server or are nav-props
            ModelState.Remove("Id");
            ModelState.Remove("Bookings");
            ModelState.Remove("Packages");
            ModelState.Remove("RegistrationDate");

            if (!ModelState.IsValid)
                return View(model);

            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
                ViewBag.Error = "Email already registered.";
                return View(model);
            }

            _context.Users.Add(model);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Registration successful! Please login.";
            return RedirectToAction("Login");
        }

        // ── Profile ────────────────────────────────────────────────────
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login");

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return RedirectToAction("Login");

            return View(user);
        }

        // ── Forgot Password ────────────────────────────────────────────
        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ViewBag.Error = "Please enter your email address.";
                return View();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                ViewBag.Error = "Email address not found.";
                return View();
            }

            TempData["SuccessMessage"] = "A password reset link has been sent to your email.";
            return RedirectToAction("Login");
        }

        // ── Edit Profile ───────────────────────────────────────────────
        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login");

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return RedirectToAction("Login");

            return View(user);
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpPost]
        public async Task<IActionResult> EditProfile(
            string Name, string Mobile, string Address,
            string Budget, int? PreferredDays, string TravelTypes, string ProfileImageUrl)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login");

            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.Name            = Name;
                user.Mobile          = Mobile;
                user.Address         = Address;
                user.Budget          = Budget;
                user.PreferredDays   = PreferredDays;
                user.TravelTypes     = TravelTypes;
                user.ProfileImageUrl = ProfileImageUrl;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Profile");
        }

        // ── Logout ─────────────────────────────────────────────────────
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // ── Access Denied ──────────────────────────────────────────────
        public IActionResult AccessDenied() => View();

        // ── Shared sign-in helper ──────────────────────────────────────
        private async Task SignInUserAsync(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name,           user.Name),
                new Claim(ClaimTypes.Email,          user.Email),
                new Claim(ClaimTypes.Role,           user.Role)
            };

            var identity  = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}
