using System.ComponentModel.DataAnnotations;

namespace Simple_Stocks.Models
{
    public class RoleRight
    {
        [Key]
        public int Id { get; set; }
        public int RightId { get; set; }
        public Right? Right { get; set; }

        public int RoleId { get; set; }
        public Role? Role { get; set; }
    }
}
