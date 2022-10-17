using System.ComponentModel.DataAnnotations;

namespace Simple_Stocks.Dtos.ModDtos
{
    public class PostVisabilityDto
    {
        [Required]
        public bool? PostIsHidden { get; set; }
    }
}
