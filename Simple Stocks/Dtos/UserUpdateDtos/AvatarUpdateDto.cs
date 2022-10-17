using System.ComponentModel.DataAnnotations;

namespace Simple_Stocks.Dtos.UserUpdateDtos
{
    public class AvatarUpdateDto
    {
        public string AvatarLink { get; set; } = string.Empty;

        public string BannerLink { get; set; } = string.Empty;
    }
}
