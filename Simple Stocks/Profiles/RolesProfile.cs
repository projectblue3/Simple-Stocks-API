using AutoMapper;
using Simple_Stocks.Dtos;
using Simple_Stocks.Models;

namespace Simple_Stocks.Profiles
{
    public class RolesProfile : Profile
    {
        public RolesProfile()
        {
            CreateMap<RoleUpdateDto, Role>();
            CreateMap<Role, RoleUpdateDto>();
        }
    }
}
