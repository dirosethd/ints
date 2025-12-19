using ints.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ints.Services
{
    public interface IAuthService
    {
        Task<User> RegisterAsync(string username, string password);
        Task<User?> LoginAsync(string username, string password);
        Task<bool> UserExistsAsync(string username);
    }

    public class AuthService : IAuthService
    {
        private readonly IDbContextFactory<AppDbContext> _factory;
        private readonly IPasswordHasher _hasher;

        public AuthService(IDbContextFactory<AppDbContext> factory, IPasswordHasher hasher)
        {
            _factory = factory;
            _hasher = hasher;
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            await using var db = await _factory.CreateDbContextAsync();
            var u = username.Trim();
            return await db.Users.AnyAsync(x => x.Username == u);
        }

        public async Task<User> RegisterAsync(string username, string password)
        {
            var u = username.Trim();

            if (string.IsNullOrWhiteSpace(u))
                throw new InvalidOperationException("Логин не может быть пустым.");
            if (password.Length < 6)
                throw new InvalidOperationException("Пароль должен быть минимум 6 символов.");

            await using var db = await _factory.CreateDbContextAsync();

            if (await db.Users.AnyAsync(x => x.Username == u))
                throw new InvalidOperationException("Пользователь с таким логином уже существует.");

            var (hash, salt) = _hasher.HashPassword(password);

            var user = new User
            {
                Username = u,
                PasswordHash = hash,
                PasswordSalt = salt
            };

            db.Users.Add(user);
            await db.SaveChangesAsync();
            return user;
        }

        public async Task<User?> LoginAsync(string username, string password)
        {
            var u = username.Trim();
            await using var db = await _factory.CreateDbContextAsync();

            var user = await db.Users.SingleOrDefaultAsync(x => x.Username == u);
            if (user == null) return null;

            return _hasher.Verify(password, user.PasswordHash, user.PasswordSalt)
                ? user
                : null;
        }
    }
}