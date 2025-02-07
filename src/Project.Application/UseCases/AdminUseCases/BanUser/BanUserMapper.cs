using AutoMapper;
using Project.Domain.Entities;

namespace Project.Application.UseCases.AdminUseCases.BanUser
{
    public sealed class BanUserMapper : Profile
    {
        public BanUserMapper()
        {
            CreateMap<BanUserRequest, User>();
            CreateMap<User, BanUserResponse>();
        }
    }

}
