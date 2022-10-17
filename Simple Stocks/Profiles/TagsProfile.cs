using AutoMapper;
using Simple_Stocks.Dtos;
using Simple_Stocks.Models;

namespace Simple_Stocks.Profiles
{
    public class TagsProfile : Profile
    {
        public TagsProfile()
        {
            CreateMap<TagUpdateDto, Tag>();
            CreateMap<Tag, TagUpdateDto>();
        }
    }
}
