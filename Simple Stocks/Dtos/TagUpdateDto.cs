using System.ComponentModel.DataAnnotations;

namespace Simple_Stocks.Dtos
{
    public class TagUpdateDto
    {
        [Required]
        [StringLength(50)]
        public string Text { get; set; } = string.Empty;
    }
}
