namespace Project.Domain.Entities
{
    public sealed class User : BaseEntity
    {
        public string Fullname { get; private set; }
        public string UserName { get; private set; }
        public string Email { get; private set; }
        public List<string> Roles { get; private set; }
        public bool IsActive { get; private set; }
        public byte[] HashPassword { get; private set; }
        public byte[] SaltPassword { get; private set; }

        // Email Confirmation
        public bool IsEmailConfirmed { get; private set; }
        public string EmailConfirmationToken { get; private set; }
        public DateTimeOffset? EmailConfirmationTokenExpiry { get; private set; }

        // Password reset
        public string PasswordResetToken { get; private set; }
        public DateTimeOffset? PasswordResetTokenExpiry { get; private set; }

        // 2FA Authentication
        public bool IsTwoFactorEnabled { get; private set; }
        public string? TwoFactorCode { get; private set; }
        public DateTimeOffset? TwoFactorCodeExpiry { get; private set; }

        // Delete account
        public bool IsDeletionRequested { get; private set; }
        public DateTimeOffset? DeletionRequestDate { get; private set; }
        public DateTimeOffset? DeletionScheduledDate { get; private set; }

        // Ban User
        public bool IsUserBanned { get; private set; }
        public DateTimeOffset? UserBannedUntil { get; private set; }

        private User() { }
        public User(string fullname, string username, string email, byte[] hashPassword, byte[] saltPassword) : this()
        {
            Fullname = fullname;
            UserName = username;
            Email = email;
            HashPassword = hashPassword;
            SaltPassword = saltPassword;
            Roles = new List<string> { "User" };
            IsActive = true;
            PasswordResetToken = string.Empty;
        }

        // update methods
        public void UpdateName(string newFullname)
        {
            Fullname = newFullname;
            UpdateDate();
        }
        public void UpdateUsername(string newUsername)
        {
            UserName = newUsername;
            UpdateDate();
        }
        public void UpdateEmail(string newEmail)
        {
            Email = newEmail;
            UpdateDate();
        }

        // roles methods
        public void AddRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
                throw new ArgumentException("Role cannot be empty.", nameof(role));

            if (!Roles.Contains(role))
            {
                Roles.Add(role);
            }
        }
        public void RemoveRole(string role)
        {
            if (Roles == null || !Roles.Contains(role))
                throw new InvalidOperationException("Role not found.");

            Roles.Remove(role);
        }

        // email confirmation methods
        public void GenerateEmailConfirmationToken()
        {
            EmailConfirmationToken = Guid.NewGuid().ToString();
            EmailConfirmationTokenExpiry = DateTimeOffset.UtcNow.AddHours(24);
        }
        public void ConfirmEmail()
        {
            IsEmailConfirmed = true;
            EmailConfirmationToken = null;
            EmailConfirmationTokenExpiry = null;
        }

        // password reset methods
        public void GeneratePasswordResetToken()
        {
            PasswordResetToken = Guid.NewGuid().ToString();
            PasswordResetTokenExpiry = DateTimeOffset.UtcNow.AddHours(1);
        }
        public bool ValidatePasswordResetToken(string token)
        {
            return PasswordResetToken == token && PasswordResetTokenExpiry > DateTimeOffset.UtcNow;
        }
        public void ResetPassword(byte[] newPasswordHash, byte[] newSalt)
        {
            HashPassword = newPasswordHash;
            SaltPassword = newSalt;
            PasswordResetToken = null;
            PasswordResetTokenExpiry = null;
        }

        // delete / deactivate / reactivate methods
        public void RequestAccountDeletion()
        {
            IsDeletionRequested = true;
            IsActive = false;
            DeletionRequestDate = DateTimeOffset.UtcNow;
            DeletionScheduledDate = DateTimeOffset.UtcNow.AddDays(30);
        }
        public void DeactivateUser()
        {
            IsActive = false;
        }
        public void ReactivateUser()
        {
            IsActive = true;
            UpdateDate();
        }
        public bool CanBeDeleted() => DeletionScheduledDate <= DateTimeOffset.UtcNow;

        // 2FA methods
        public void EnableTwoFactorAuthentication()
        {
            IsTwoFactorEnabled = true;
        }
        public void DisableTwoFactorAuthentication()
        {
            IsTwoFactorEnabled = false;
            TwoFactorCode = null;
            TwoFactorCodeExpiry = null;
        }
        public void GenerateTwoFactorCode()
        {
            TwoFactorCode = new Random().Next(100000, 999999).ToString();
            TwoFactorCodeExpiry = DateTimeOffset.UtcNow.AddMinutes(10);
        }
        public bool ValidateTwoFactorCode(string code)
        {
            return TwoFactorCode == code && TwoFactorCodeExpiry > DateTimeOffset.UtcNow;
        }

        // Ban & Unban methods
        public void BanUser(DateTimeOffset? until = null)
        {
            IsUserBanned = true;
            UserBannedUntil = until;
        }
        public void UnbanUser()
        {
            IsUserBanned = false;
            UserBannedUntil = null;
        }
    }
}