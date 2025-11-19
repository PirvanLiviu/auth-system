using System.Security.Cryptography;
using System.Text;

namespace NeonTest.Services;

public static class PasswordService
{
  public static string HashPassword(string password)
  {
    using var sha256 = SHA256.Create();
    var bytes = Encoding.UTF8.GetBytes(password);
    var hash = sha256.ComputeHash(bytes);
    return Convert.ToBase64String(hash);
  }

  public static bool VerifyPassword(string password, string hashPassword)
  {
    return HashPassword(password) == hashPassword;
  }
}
