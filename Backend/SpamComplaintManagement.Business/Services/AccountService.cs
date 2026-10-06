using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SpamComplaintManagement.Data.Data;
using SpamComplaintManagement.Data.Models;

namespace SpamComplaintManagement.Business.Services;

public class AccountService : IAccountService
{
    private readonly ApplicationDbContext _db;
    private readonly PasswordHasher<LoginCredential> _passwordHasher;

    public AccountService(ApplicationDbContext db)
    {
        _db = db;
        _passwordHasher = new PasswordHasher<LoginCredential>();
    }

    public RegisterResult Register(
        string fullName,
        string email,
        string password)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            return new RegisterResult
            {
                Success = false,
                Message = "Full name is required."
            };
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            return new RegisterResult
            {
                Success = false,
                Message = "Email is required."
            };
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            return new RegisterResult
            {
                Success = false,
                Message = "Password is required."
            };
        }

        email = email.Trim().ToLowerInvariant();

        var existingUser = _db.LoginCredentials
            .FirstOrDefault(x => x.Email == email);

        if (existingUser != null)
        {
            return new RegisterResult
            {
                Success = false,
                Message = "An account with this email already exists."
            };
        }

        var user = new LoginCredential
        {
            FullName = fullName.Trim(),
            Email = email,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(
            user,
            password);

        _db.LoginCredentials.Add(user);
        _db.SaveChanges();

        return new RegisterResult
        {
            Success = true,
            Message = "Registration successful.",
            User = user
        };
    }

    public LoginResult Login(
        string email,
        string password)
    {
        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            return new LoginResult
            {
                Success = false,
                Message = "Email and password are required."
            };
        }

        email = email.Trim().ToLowerInvariant();

        var user = _db.LoginCredentials
            .FirstOrDefault(x => x.Email == email);

        if (user == null)
        {
            return new LoginResult
            {
                Success = false,
                Message = "Invalid email or password."
            };
        }

        var passwordResult =
            _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                password);

        if (passwordResult == PasswordVerificationResult.Failed)
        {
            return new LoginResult
            {
                Success = false,
                Message = "Invalid email or password."
            };
        }

        return new LoginResult
        {
            Success = true,
            Message = "Login successful.",
            User = user
        };
    }
}