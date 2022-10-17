using System.ComponentModel.DataAnnotations;

namespace Simple_Stocks.Models
{
    public class Role
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = String.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public virtual ICollection<RoleRight>? RoleRights { get; set; }

        public virtual ICollection<User>? Users { get; set; }
    }
}
