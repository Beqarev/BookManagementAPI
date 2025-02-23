using AutoMapper;
using BookManagementAPI.BLL.Models.Dtos;
using BookManagementAPI.DAL.Domain;

namespace BookManagementAPI.BLL.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Book, GetBookDto>();
        CreateMap<CreateBookRequest, Book>();
        CreateMap<UpdateBookRequest, Book>()
            .ForMember(dest => dest.CreateDate, opt => opt.Ignore())
            .ForMember(dest => dest.DeleteDate, opt => opt.Ignore());
    }
}