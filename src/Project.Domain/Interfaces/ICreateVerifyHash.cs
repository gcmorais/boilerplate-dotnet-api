namespace Project.Domain.Interfaces
{
    public interface ICreateVerifyHash
    {
        void CreateHashPassword(string password, out byte[] hashPassword, out byte[] saltPassword);
        bool PasswordVerify(string password, byte[] hashPassword, byte[] saltPassword);
    }
}
