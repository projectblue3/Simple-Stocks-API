using Simple_Stocks.Models;
using Simple_Stocks.Utils;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Simple_Stocks.Dtos
{
    public class RegisterDto
    {
        public string AvatarLink { get; set; } = string.Empty;

        public string BannerLink { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        //regex username for no spaces
        public string Username { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{8,}$", ErrorMessage = "Password must include an uppercase letter, lowercase letter, special charactor, and a number")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{4}-(0[1-9]|1[0-2])-(0[1-9]|[12][0-9]|3[01])$", ErrorMessage = "Date format should be YYYY-MM-DD")]
        public string DateOfBirth { get; set; } = string.Empty;

        [Phone]
        public string PhoneNumber { get; set; } = String.Empty;

        [StringLength(500)]
        public string Bio { get; set; } = string.Empty;

        public int RoleId { get; set; }
        public Role? Role { get; set; }
    }
}
