using AutoMapper;
using Simple_Stocks.Dtos;
using Simple_Stocks.Dtos.ModDtos;
using Simple_Stocks.Models;
namespace Simple_Stocks.Profiles
{
    public class CommentsProfile : Profile
    {
        public CommentsProfile()
        {
            CreateMap<CommentUpdateDto, Comment>();
            CreateMap<Comment, CommentUpdateDto>();
            CreateMap<CommentVisabilityDto, Comment>();
            CreateMap<Comment, CommentVisabilityDto>();
        }
    }
}
