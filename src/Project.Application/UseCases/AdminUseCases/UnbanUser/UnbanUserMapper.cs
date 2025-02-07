using AutoMapper;
using Project.Domain.Entities;

namespace Project.Application.UseCases.AdminUseCases.UnbanUser
{
    public sealed class UnbanUserMapper : Profile
    {
        public UnbanUserMapper()
        {
            CreateMap<UnbanUserRequest, User>();
            CreateMap<User, UnbanUserResponse>();
        }
    }
}
