using Application.Interfaces.Services;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services;

/// <summary>
/// Service for password hashing and verification using BCrypt
/// </summary>
public class PasswordService : IPasswordService
{
    /// <summary>
    /// Hash a plain text password using BCrypt
    /// </summary>
    /// <param name="password">Plain text password</param>
    /// <returns>Hashed password</returns>
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
    
    /// <summary>
    /// Verify a plain text password against a BCrypt hash
    /// </summary>
    /// <param name="password">Plain text password</param>
    /// <param name="hashedPassword">BCrypt hashed password</param>
    /// <returns>True if password matches, false otherwise</returns>
    public bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}