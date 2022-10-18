using Simple_Stocks.Models;
using Simple_Stocks.Utils;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Simple_Stocks.Dtos
{
    public class LoginDto
    {
        [Required]
        //[StringLength(50)]
        //regex username for no spaces
        public string Username { get; set; } = string.Empty;

        [Required]
        //[MinLength(8)]
        //[RegularExpression(@"^(?=.*[A-Z])(?=.*[!@#$&*])(?=.*[0-9])(?=.*[a-z])$", ErrorMessage = "Password must include an uppercase letter, lowercase letter, special charactor (!@#$&*), and a number")]
        public string Password { get; set; } = string.Empty;
    }
}
