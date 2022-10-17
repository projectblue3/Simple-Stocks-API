using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Simple_Stocks.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(3000)]
        public string Text { get; set; } = string.Empty;

        [Required]
        public bool? CommentIsHidden { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        //n:n
        public virtual ICollection<LikedComment>? LikedComments { get; set; }

        //n:1
        [ForeignKey("User")]
        public int UserID { get; set; }
        public virtual User? User { get; set; }

        //n:1
        [ForeignKey("post")]
        public int PostId { get; set; }
        public virtual Post? Post { get; set; }
    }
}
