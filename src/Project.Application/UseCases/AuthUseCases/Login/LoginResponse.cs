namespace Project.Application.UseCases.AuthUseCases.Login
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public bool RequiresTwoFactor { get; set; }
    }
}
