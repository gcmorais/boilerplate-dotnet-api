using AutoMapper;
using Project.Application.UseCases.UserUseCases.Common;
using Project.Domain.Entities;

namespace Project.Application.UseCases.UserUseCases.Create
{
    public sealed class UserCreateMapper : Profile
    {
        public UserCreateMapper()
        {
            CreateMap<UserCreateRequest, User>();
            CreateMap<User, UserResponse>();
        }
    }
}
