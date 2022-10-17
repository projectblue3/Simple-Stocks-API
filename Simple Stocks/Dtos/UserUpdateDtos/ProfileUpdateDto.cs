using System.ComponentModel.DataAnnotations;

namespace Simple_Stocks.Dtos.UserUpdateDtos
{
    public class ProfileUpdateDto
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [StringLength(500)]
        public string Bio { get; set; } = string.Empty;
    }
}
