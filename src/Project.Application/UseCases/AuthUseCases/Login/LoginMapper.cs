using AutoMapper;
using Project.Domain.Entities;

namespace Project.Application.UseCases.AuthUseCases.Login
{
    public sealed class LoginMapper : Profile
    {
        public LoginMapper()
        {
            CreateMap<LoginUserRequest, User>();
            CreateMap<User, LoginResponse>();
        }
    }
}
