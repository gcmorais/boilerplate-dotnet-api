using AutoMapper;
using Project.Application.UseCases.UserUseCases.Common;
using Project.Domain.Entities;

namespace Project.Application.UseCases.UserUseCases.Update
{
    public sealed class UserUpdateMapper : Profile
    {
        public UserUpdateMapper()
        {
            CreateMap<UserUpdateRequest, User>();
            CreateMap<User, UserResponse>();
        }
    }
}
