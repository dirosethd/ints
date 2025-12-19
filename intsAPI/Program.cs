using ints;
using ints.Models;

using intsAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "intsAPI", Version = "v1" });

    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Введите JWT токен. Пример: Bearer {token}",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            },
            Array.Empty<string>()
        }
    });
});

// DbContext
string cs = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddDbContext<IntsContext>(opt => opt.UseSqlServer(cs));

// JWT Auth
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = AuthOptions.ISSUER,

            ValidateAudience = true,
            ValidAudience = AuthOptions.AUDIENCE,

            ValidateLifetime = true,

            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(builder.Configuration),
            ValidateIssuerSigningKey = true
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


// ================== LOGIN ==================
app.MapPost("/login", async (LoginRequest req, IntsContext db) =>
{
    var user = await db.Users.FirstOrDefaultAsync(u => u.Username == req.Username);
    if (user == null)
        return Results.Unauthorized();

    if (!PasswordHelper.VerifyPassword(
            req.Password,
            user.PasswordHash,
            user.PasswordSalt))
    {
        return Results.Unauthorized();
    }

    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
    };

    var jwt = new JwtSecurityToken(
        issuer: AuthOptions.ISSUER,
        audience: AuthOptions.AUDIENCE,
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(30),
        signingCredentials: new SigningCredentials(
            AuthOptions.GetSymmetricSecurityKey(builder.Configuration),
            SecurityAlgorithms.HmacSha256));

    var token = new JwtSecurityTokenHandler().WriteToken(jwt);

    return Results.Json(new
    {
        access_token = token,
        username = user.Username
    });
});


// ================== REGISTER ==================
app.MapPost("/register", async (RegisterRequest req, IntsContext db) =>
{
    if (string.IsNullOrWhiteSpace(req.Username) ||
        string.IsNullOrWhiteSpace(req.Password))
    {
        return Results.BadRequest("Username и Password обязательны");
    }

    bool exists = await db.Users.AnyAsync(u => u.Username == req.Username);
    if (exists)
        return Results.BadRequest("Пользователь уже существует");

    var result = PasswordHelper.HashPassword(req.Password);

    var user = new User
    {
        Username = req.Username,
        PasswordHash = result.hash,
        PasswordSalt = result.salt
    };

    db.Users.Add(user);
    await db.SaveChangesAsync();

    return Results.Ok(new
    {
        user.Id,
        user.Username
    });
});

app.MapControllers();
app.Run();


// ================== DTO ==================
public record LoginRequest(string Username, string Password);
public record RegisterRequest(string Username, string Password);


// ================== JWT OPTIONS ==================
public static class AuthOptions
{
    public const string ISSUER = "MyAuthServer";
    public const string AUDIENCE = "MyAuthClient";

    public static SymmetricSecurityKey GetSymmetricSecurityKey(IConfiguration config)
    {
        string key = config["Jwt:Key"] ?? throw new Exception("Jwt:Key отсутствует в appsettings.json");
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    }
}

