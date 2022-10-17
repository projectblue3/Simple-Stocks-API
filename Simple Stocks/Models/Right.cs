using System.ComponentModel.DataAnnotations;

namespace Simple_Stocks.Models
{
    public class Right
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public virtual ICollection<RoleRight>? RoleRights { get; set; }
    }
}
