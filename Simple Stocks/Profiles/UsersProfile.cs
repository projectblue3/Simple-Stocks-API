using AutoMapper;
using Simple_Stocks.Dtos.ModDtos;
using Simple_Stocks.Dtos.PrivacyDtos;
using Simple_Stocks.Dtos.UserUpdateDtos;
using Simple_Stocks.Models;

namespace Simple_Stocks.Profiles
{
    public class UsersProfile : Profile
    {
        public UsersProfile()
        {
            CreateMap<UserVisabilityDto, User>();
            CreateMap<User, UserVisabilityDto>();

            CreateMap<AvatarUpdateDto, User>();
            CreateMap<User, AvatarUpdateDto>();

            CreateMap<ContactUpdateDto, User>();
            CreateMap<User, ContactUpdateDto>();

            CreateMap<PasswordUpdateDto, User>();
            CreateMap<User, PasswordUpdateDto>();

            CreateMap<UserPrivacyDto, User>();
            CreateMap<User, UserPrivacyDto>();

            CreateMap<ProfileUpdateDto, User>();
            CreateMap<User, ProfileUpdateDto>();
        }
    }
}
