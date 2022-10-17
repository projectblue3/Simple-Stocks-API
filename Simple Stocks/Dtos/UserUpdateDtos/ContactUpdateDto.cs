using System.ComponentModel.DataAnnotations;

namespace Simple_Stocks.Dtos.UserUpdateDtos
{
    public class ContactUpdateDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string PhoneNumber { get; set; } = String.Empty;
    }
}
