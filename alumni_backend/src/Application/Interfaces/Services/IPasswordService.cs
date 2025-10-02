namespace Application.Interfaces.Services;

/// <summary>
/// Interface for password hashing and verification operations
/// </summary>
public interface IPasswordService
{
    /// <summary>
    /// Hash a plain text password
    /// </summary>
    /// <param name="password">Plain text password</param>
    /// <returns>Hashed password</returns>
    string HashPassword(string password);
    
    /// <summary>
    /// Verify a plain text password against a hash
    /// </summary>
    /// <param name="password">Plain text password</param>
    /// <param name="hashedPassword">Hashed password to verify against</param>
    /// <returns>True if password matches, false otherwise</returns>
    bool VerifyPassword(string password, string hashedPassword);
}