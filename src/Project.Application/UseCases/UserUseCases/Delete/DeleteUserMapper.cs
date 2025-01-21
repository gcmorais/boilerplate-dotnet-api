using AutoMapper;
using Project.Application.UseCases.UserUseCases.Common;
using Project.Domain.Entities;

namespace Project.Application.UseCases.UserUseCases.Delete
{
    public sealed class DeleteUserMapper : Profile
    {
        public DeleteUserMapper()
        {
            CreateMap<DeleteUserRequest, User>();
            CreateMap<User, UserResponse>();
        }
    }
}
