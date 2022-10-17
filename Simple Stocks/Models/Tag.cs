using System.ComponentModel.DataAnnotations;

namespace Simple_Stocks.Models
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Text { get; set; } = string.Empty;

        //n:n
        public virtual ICollection<PostTag>? PostTags { get; set; }
    }
}
