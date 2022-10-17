using AutoMapper;
using Simple_Stocks.Dtos;
using Simple_Stocks.Dtos.ModDtos;
using Simple_Stocks.Dtos.PrivacyDtos;
using Simple_Stocks.Models;

namespace Simple_Stocks.Profiles
{
    public class PostsProfile : Profile
    {
        public PostsProfile()
        {
            CreateMap<PostUpdateDto, Post>();
            CreateMap<Post, PostUpdateDto>();
            CreateMap<PostPrivacyDto, Post>();
            CreateMap<Post, PostPrivacyDto>();
            CreateMap<PostVisabilityDto, Post>();
            CreateMap<Post, PostVisabilityDto>();
        }
    }
}
