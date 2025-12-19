using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ints.Services
{
    public interface IPasswordHasher
    {
        (byte[] hash, byte[] salt) HashPassword(string password);
        bool Verify(string password, byte[] expectedHash, byte[] salt);
    }

    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 100_000;

        public (byte[] hash, byte[] salt) HashPassword(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(SaltSize);

            using var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256);

            var hash = pbkdf2.GetBytes(KeySize);
            return (hash, salt);
        }

        public bool Verify(string password, byte[] expectedHash, byte[] salt)
        {
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