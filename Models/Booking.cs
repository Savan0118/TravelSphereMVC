using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelSphereMVC.Models
{
    public class Booking
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        [Required]
        public string PackageId { get; set; } = string.Empty;

        [ForeignKey("PackageId")]
        public Package Package { get; set; } = null!;

        public string Status { get; set; } = "Pending";

        public decimal TotalAmount { get; set; }

        public string? SpecialRequests { get; set; }

        public ICollection<Traveller> Travellers { get; set; } = new List<Traveller>();
    }
}
