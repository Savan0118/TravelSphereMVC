using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelSphereMVC.Models
{
    public class Traveller
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string Name { get; set; } = string.Empty;

        public int Age { get; set; }

        /// <summary>Adult | Child</summary>
        public string TravellerType { get; set; } = "Adult";

        public string BookingId { get; set; } = string.Empty;

        [ForeignKey("BookingId")]
        public Booking Booking { get; set; } = null!;
    }
}
