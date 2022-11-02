using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Simple_Stocks.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(128)]
        public string Title { get; set; } = string.Empty;

        public string MediaLink { get; set; } = string.Empty;

        public string MediaType { get; set; } = string.Empty;

        [Required]
        [StringLength(3000)]
        public string Text { get; set; } = string.Empty;

        [Required]
        public bool? PostIsHidden { get; set; }

        [Required]
        public bool? PostIsPrivate { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        //n:n
        public virtual ICollection<PostTag>? PostTags { get; set; }

        //n:n
        public virtual ICollection<LikedPost>? LikedPosts { get; set; }

        //1:n
        public virtual ICollection<Comment>? Comments { get; set; }

        //n:1
        [ForeignKey("User")]
        public int UserID { get; set; }

        public string Author { get; set; } = string.Empty;
        public virtual User? User { get; set; }
    }
}
