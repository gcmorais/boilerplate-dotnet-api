using Project.Domain.Entities;

namespace Project.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user, List<string> roles);
    }
}
