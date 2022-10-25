using Simple_Stocks.Utils;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Simple_Stocks.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string AvatarLink { get; set; } = string.Empty;

        public string BannerLink { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        //regex username for no spaces
        public string Username { get; set; } = string.Empty;

        //[Required]
        //[MinLength(8)]
        //[RegularExpression(@"^(?=.*[A-Z])(?=.*[!@#$&*])(?=.*[0-9])(?=.*[a-z])$", ErrorMessage = "Password must include an uppercase letter, lowercase letter, special charactor (!@#$&*), and a number")]
        public string Password { get; set; } = string.Empty;

        [Required]
        public byte[] PasswordHash { get; set; }

        [Required]
        public byte[] PasswordSalt { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{4}-(0[1-9]|1[0-2])-(0[1-9]|[12][0-9]|3[01])$", ErrorMessage = "Date format should be YYYY-MM-DD")]
        public string DateOfBirth { get; set; } = string.Empty;

        [Phone]
        public string PhoneNumber { get; set; } = String.Empty;

        [StringLength(500)]
        public string Bio { get; set; } = string.Empty;

        [Required]
        public bool? AccountIsEnabled { get; set; }

        [Required]
        public bool? AccountIsHidden { get; set; }

        [Required]
        public bool? AccountIsPrivate { get; set; }

        [Required]
        public bool? LikesArePrivate { get; set; }

        [Required]
        public bool? FollowsArePrivate { get; set; }

        public int RoleId { get; set; }
        public Role? Role { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        //1:1
        public virtual RefreshToken? RefreshToken { get; set; }

        //1:n
        public virtual ICollection<Post>? Posts { get; set; }

        //1:n
        public virtual ICollection<Comment>? Comments { get; set; }

        //n:n
        public virtual ICollection<UserStock>? UserStocks { get; set; }

        //n:n
        public virtual ICollection<UserFollow>? Following { get; set; }
        public virtual ICollection<UserFollow>? Followers { get; set; }
        public virtual ICollection<UserBlock>? BlockedUsers { get; set; }

        //n:n
        public virtual ICollection<LikedPost>? LikedPosts { get; set; }

        //n:n
        public virtual ICollection<LikedComment>? LikedComments { get; set; }
    }
}
