namespace Project.Application.UseCases.UserUseCases.Common
{
    public sealed record UserResponse
    {
        public Guid Id { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? UserName { get; set; }
        public byte[] HashPassword { get; set; }
        public byte[] SaltPassword { get; set; }
        public List<string> Roles { get; set; }
        public bool IsActive { get; set; }
        public bool? IsEmailConfirmed { get; set; }
        public bool? IsTwoFactorEnabled { get; set; }
        public bool? IsDeletionRequested { get; set; }
        public DateTimeOffset? DeletionRequestDate { get; set; }
        public DateTimeOffset? DeletionScheduledDate { get; set; }
    }
}
