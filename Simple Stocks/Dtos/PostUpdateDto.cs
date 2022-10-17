using System.ComponentModel.DataAnnotations;

namespace Simple_Stocks.Dtos
{
    public class PostUpdateDto
    {
        [Required]
        [StringLength(128)]
        public string Title { get; set; } = string.Empty;

        public string MediaLink { get; set; } = string.Empty;

        public string MediaType { get; set; } = string.Empty;

        [Required]
        [StringLength(3000)]
        public string Text { get; set; } = string.Empty;
    }
}
