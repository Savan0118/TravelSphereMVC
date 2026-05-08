using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelSphereMVC.Models
{
    public class Package
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        // ── Basic Details ──────────────────────────────────────────────
        [Required]
        public string Name { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public string? Country { get; set; }
        public string? State { get; set; }

        /// <summary>Beach | Mountain | Heritage | Adventure | Wildlife | City | Pilgrimage</summary>
        public string? DestinationType { get; set; }

        public string? ShortDescription { get; set; }
        public string? FullDescription  { get; set; }

        // ── Travel Details ─────────────────────────────────────────────
        public string Duration { get; set; } = string.Empty;

        public string? PickupPoint     { get; set; }
        public string? DropPoint       { get; set; }
        public string? BestSeason      { get; set; }

        /// <summary>Solo | Family | Group | Honeymoon | Corporate</summary>
        public string? TravelType      { get; set; }

        /// <summary>Easy | Moderate | Challenging</summary>
        public string? DifficultyLevel { get; set; }

        // ── Hotel & Food ───────────────────────────────────────────────
        public string? HotelName      { get; set; }
        public int?    HotelRating    { get; set; }   // 1–5

        /// <summary>Standard | Deluxe | Suite | Dormitory</summary>
        public string? RoomType       { get; set; }

        /// <summary>None | Breakfast | Half Board | All Meals</summary>
        public string? MealsIncluded  { get; set; }

        // ── Pricing ────────────────────────────────────────────────────
        public decimal  Price              { get; set; }
        public decimal? DiscountPercentage { get; set; }

        public int TotalSeats { get; set; } = 15;

        // ── Media ──────────────────────────────────────────────────────
        public string? ImageUrl      { get; set; }

        /// <summary>Comma-separated gallery image URLs</summary>
        public string? GalleryImages { get; set; }

        // ── Content ────────────────────────────────────────────────────
        public string? ActivitiesList    { get; set; }
        public string? Itinerary         { get; set; }
        public string? SpecialAttractions{ get; set; }

        // ── Admin Ownership ────────────────────────────────────────────
        public string? AdminId { get; set; }

        [ForeignKey("AdminId")]
        public User? Admin { get; set; }

        // ── Navigation ─────────────────────────────────────────────────
        public ICollection<Booking>      Bookings      { get; set; } = new List<Booking>();
        public ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
        public ICollection<Review>       Reviews       { get; set; } = new List<Review>();
    }
}
