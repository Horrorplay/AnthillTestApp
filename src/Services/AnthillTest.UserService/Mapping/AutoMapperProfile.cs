using AnthillTest.UserService.DTO;
using AnthillTest.UserService.Entities;
using AutoMapper;

namespace AnthillTest.UserService.Mapping;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<UserDto, User>();
        CreateMap<User, UserDto>();
    }
}
