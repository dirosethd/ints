using System.Security.Cryptography;

namespace intsAPI.Services
{
    public static class PasswordHelper
    {
        public static (byte[] hash, byte[] salt) HashPassword(string password)
        {
            const int SaltSize = 16;
            const int KeySize = 32;
            const int Iterations = 100_000;

            var salt = RandomNumberGenerator.GetBytes(SaltSize);

            using var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256);

            var hash = pbkdf2.GetBytes(KeySize);
            return (hash, salt);
        }

        public static bool VerifyPassword(string password, byte[] expectedHash, byte[] salt)
        {
            const int KeySize = 32;
            const int Iterations = 100_000;

            using var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256);

            var actualHash = pbkdf2.GetBytes(KeySize);
            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }
    }
}