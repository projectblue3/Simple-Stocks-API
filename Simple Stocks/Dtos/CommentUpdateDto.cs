using System.ComponentModel.DataAnnotations;

namespace Simple_Stocks.Dtos
{
    public class CommentUpdateDto
    {
        [Required]
        [StringLength(3000)]
        public string Text { get; set; } = string.Empty;
    }
}
