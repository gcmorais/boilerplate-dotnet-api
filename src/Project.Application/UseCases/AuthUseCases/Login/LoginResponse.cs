namespace Project.Application.UseCases.AuthUseCases.Login
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public bool RequiresTwoFactor { get; set; }
    }
}
