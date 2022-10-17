using System.ComponentModel.DataAnnotations;

namespace Simple_Stocks.Dtos.UserUpdateDtos
{
    public class PasswordUpdateDto
    {
        [Required]
        [MinLength(8)]
        //[RegularExpression(@"^(?=.*[A-Z])(?=.*[!@#$&*])(?=.*[0-9])(?=.*[a-z])$", ErrorMessage = "Password must include an uppercase letter, lowercase letter, special charactor (!@#$&*), and a number")]
        public string Password { get; set; } = string.Empty;
    }
}
