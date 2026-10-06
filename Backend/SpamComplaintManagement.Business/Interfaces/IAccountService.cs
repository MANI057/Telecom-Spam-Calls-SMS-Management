using SpamComplaintManagement.Data.Models;

namespace SpamComplaintManagement.Business.Services;

public interface IAccountService
{
    RegisterResult Register(
        string fullName,
        string email,
        string password);

    LoginResult Login(
        string email,
        string password);
}

public class RegisterResult
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public LoginCredential? User { get; set; }
}

public class LoginResult
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public LoginCredential? User { get; set; }
}