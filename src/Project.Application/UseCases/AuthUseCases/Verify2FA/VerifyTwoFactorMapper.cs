using AutoMapper;
using Project.Application.UseCases.AuthUseCases.Login;
using Project.Domain.Entities;

namespace Project.Application.UseCases.AuthUseCases.Verify2FA
{
    public class VerifyTwoFactorMapper : Profile
    {
        public VerifyTwoFactorMapper()
        {
            CreateMap<User, LoginResponse>();
        }
    }
}
