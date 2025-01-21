using AutoMapper;
using Project.Application.UseCases.UserUseCases.Common;
using Project.Domain.Entities;

namespace Project.Application.UseCases.UserUseCases.GetById
{
    public sealed class GetByIdUserMapper : Profile
    {
        public GetByIdUserMapper()
        {
            CreateMap<GetByIdUserRequest, User>();
            CreateMap<User, UserResponse>();
        }
    }
}
