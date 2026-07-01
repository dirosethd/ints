using ints.Models;
using intsAPI.DTOs;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace intsAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IntsContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(IntsContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null || !PasswordHelper.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt))
                throw new UnauthorizedAccessException("Invalid username or password");

            var token = GenerateJwtToken(user);

            return new AuthResponse
            {
                AccessToken = token,
                Username = user.Username,
                UserId = user.Id
            };
        }

        public async Task<UserDto> RegisterAsync(RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Username and password are required");

            var exists = await _context.Users.AnyAsync(u => u.Username == request.Username);
            if (exists)
                throw new InvalidOperationException("Username already exists");

            var (hash, salt) = PasswordHelper.HashPassword(request.Password);

            var user = new User
            {
                Username = request.Username,
                PasswordHash = hash,
                PasswordSalt = salt
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                CreatedAtUtc = user.CreatedAtUtc
            };
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured")));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "MyAuthServer",
                audience: "MyAuthClient",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
