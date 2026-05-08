using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelSphereMVC.Models
{
    public class Review
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        [Required]
        public string PackageId { get; set; } = string.Empty;

        [ForeignKey("PackageId")]
        public Package Package { get; set; } = null!;

        [Range(1, 5)]
        public int Rating { get; set; } = 5;

        [Required]
        public string Comment { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
