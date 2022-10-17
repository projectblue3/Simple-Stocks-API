using System.ComponentModel.DataAnnotations;

namespace Simple_Stocks.Dtos
{
    public class RightUpdateDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;
    }
}
