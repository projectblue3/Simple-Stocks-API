using System.ComponentModel.DataAnnotations;

namespace Simple_Stocks.Dtos.ModDtos
{
    public class UserVisabilityDto
    {
        [Required]
        public bool? AccountIsEnabled { get; set; }

        [Required]
        public bool? AccountIsHidden { get; set; }

        public int RoleId { get; set; }
    }
}
