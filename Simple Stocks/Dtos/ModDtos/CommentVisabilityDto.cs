using System.ComponentModel.DataAnnotations;

namespace Simple_Stocks.Dtos.ModDtos
{
    public class CommentVisabilityDto
    {
        [Required]
        public bool? CommentIsHidden { get; set; }
    }
}
