using System.ComponentModel.DataAnnotations;

namespace Simple_Stocks.Dtos.PrivacyDtos
{
    public class PostPrivacyDto
    {
        [Required]
        public bool? PostIsPrivate { get; set; }
    }
}
