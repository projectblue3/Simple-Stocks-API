using System.ComponentModel.DataAnnotations;

namespace Simple_Stocks.Dtos
{
    public class RoleUpdateDto
    {
        [Required]
        public string Title { get; set; } = String.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;
    }
}
