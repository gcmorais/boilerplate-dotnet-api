using AutoMapper;
using Project.Application.UseCases.UserUseCases.Common;
using Project.Domain.Entities;

namespace Project.Application.UseCases.UserUseCases.GetAll
{
    public sealed class GetAllUserMapper : Profile
    {
        public GetAllUserMapper()
        {
            CreateMap<GetAllUserRequest, User>();
            CreateMap<User, UserShortResponse>();
        }
    }
}
