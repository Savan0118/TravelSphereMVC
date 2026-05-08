using System.ComponentModel.DataAnnotations;

namespace TravelSphereMVC.Models
{
    public class User
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Mobile { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public string Role { get; set; } = "Traveller";

        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        // Profile Extended Fields
        public string? Address { get; set; }
        public string? Budget { get; set; }
        public int? PreferredDays { get; set; }
        public string? TravelTypes { get; set; }
        public string? ProfileImageUrl { get; set; }

        // Navigation — Traveller side
        public ICollection<Booking>      Bookings      { get; set; } = new List<Booking>();
        public ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public ICollection<Review>       Reviews       { get; set; } = new List<Review>();

        // Navigation — Admin side
        public ICollection<Package> Packages { get; set; } = new List<Package>();
    }
}
