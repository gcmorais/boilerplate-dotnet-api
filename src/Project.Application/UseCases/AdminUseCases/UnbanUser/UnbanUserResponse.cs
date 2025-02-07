namespace Project.Application.UseCases.AdminUseCases.UnbanUser
{
    public class UnbanUserResponse
    {
        public Guid UserId { get; set; }
        public bool IsBanned { get; set; }
        public DateTimeOffset? BannedUntil { get; set; }
        public string Description { get; set; }
    }
}
