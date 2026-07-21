using Coding.Models;
using Microsoft.AspNetCore.Identity;

namespace Coding.Infrastructure.Authentication;

/// <summary>Uses ASP.NET Core Identity hashing while retaining verification for legacy BCrypt accounts.</summary>
public sealed class IdentityPasswordService
{
    private readonly PasswordHasher<User> hasher = new();

    public string Hash(User user, string password) => hasher.HashPassword(user, password);

    public bool Verify(User user, string password)
    {
        if (user.PasswordHash.StartsWith("$2", StringComparison.Ordinal))
            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        return hasher.VerifyHashedPassword(user, user.PasswordHash, password) != PasswordVerificationResult.Failed;
    }
}
