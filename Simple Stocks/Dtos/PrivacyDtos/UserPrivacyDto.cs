using System.ComponentModel.DataAnnotations;

namespace Simple_Stocks.Dtos.PrivacyDtos
{
    public class UserPrivacyDto
    {
        [Required]
        public bool? AccountIsPrivate { get; set; }

        [Required]
        public bool? LikesArePrivate { get; set; }

        [Required]
        public bool? FollowsArePrivate { get; set; }
    }
}
