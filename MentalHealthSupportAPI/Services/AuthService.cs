using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MentalHealthSupportAPI.Data;
using MentalHealthSupportAPI.DTOs;
using MentalHealthSupportAPI.Models;

namespace MentalHealthSupportAPI.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    // Registrer ny bruger
    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        // Tjek om brugernavnet allerede eksisterer
        if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            return null;

        var user = new User
        {
            Username = request.Username,
            // BCrypt hasher passwordet
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new AuthResponse
        {
            Token = GenerateJwtToken(user),
            Username = user.Username,
            Role = user.Role
        };
    }

    // Log ind eksisterende bruger
    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        // Sammenligner plaintext med hashet
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;

        return new AuthResponse
        {
            Token = GenerateJwtToken(user),
            Username = user.Username,
            Role = user.Role
        };
    }

    // Genererer JWT token baseret på brugerens information
    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

        // Signerer tokenet med HMAC SHA256 algoritmen og kan ikke ændres uden nøglen
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Claims - info der "pakkes ind" i tokenet
        // Modtageren kan verificere at de er ægte via signaturen
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8), // Token udløber efter 8 timer
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}