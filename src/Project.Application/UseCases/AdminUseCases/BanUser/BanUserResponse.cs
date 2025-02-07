namespace Project.Application.UseCases.AdminUseCases.BanUser
{
    public class BanUserResponse
    {
        public Guid UserId { get; set; }
        public bool IsBanned { get; set; }
        public DateTimeOffset? BannedUntil { get; set; }
    }
}
