using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelSphereMVC.Models
{
    public class WishlistItem
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

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
