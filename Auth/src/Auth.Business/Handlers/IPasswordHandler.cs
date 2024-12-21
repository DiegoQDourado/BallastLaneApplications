namespace Auth.Business.Handlers
{
    public interface IPasswordHandler
    {
        string HashPassword(string password);

        bool VerifyPassword(string password, string hashedPassword);
    }
}
