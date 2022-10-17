using AutoMapper;
using Simple_Stocks.Dtos;
using Simple_Stocks.Models;

namespace Simple_Stocks.Profiles
{
    public class RightsProfile : Profile
    {
        public RightsProfile()
        {
            CreateMap<RightUpdateDto, Right>();
            CreateMap<Right, RightUpdateDto>();
        }
    }
}
