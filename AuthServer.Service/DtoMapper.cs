using AuthServer.Core.Dtos;
using AuthServer.Core.Entities.Concrete;
using AutoMapper;

namespace AuthServer.Service
{
    class DtoMapper : Profile
    {
        public DtoMapper()
        {
            CreateMap<ProductDto, Product>().ReverseMap();
            CreateMap<UserDto, User>().ReverseMap();
        }
    }
}
