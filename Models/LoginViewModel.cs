using System.ComponentModel.DataAnnotations;

namespace TravelSphereMVC.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string PasswordHash { get; set; } = string.Empty;

        public string Role { get; set; } = "Traveller";
    }
}
