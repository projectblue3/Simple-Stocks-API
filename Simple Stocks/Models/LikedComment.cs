using System.ComponentModel.DataAnnotations;

namespace Simple_Stocks.Models
{
    public class LikedComment
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }

        public int CommentId { get; set; }
        public Comment? Comment { get; set; }
    }
}