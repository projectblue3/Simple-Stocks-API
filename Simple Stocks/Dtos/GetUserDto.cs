using Simple_Stocks.Models;
using Simple_Stocks.Utils;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Simple_Stocks.Dtos
{
    public class GetUserDto
    {
        public int Id { get; set; }

        public string AvatarLink { get; set; } = string.Empty;

        public string BannerLink { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;
    
        public string Bio { get; set; } = string.Empty;

        public bool? AccountIsEnabled { get; set; }

        public bool? AccountIsHidden { get; set; }

        public bool? AccountIsPrivate { get; set; }

        public bool? LikesArePrivate { get; set; }

        public bool? FollowsArePrivate { get; set; }

        public int RoleId { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}
