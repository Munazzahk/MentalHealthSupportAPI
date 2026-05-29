using Microsoft.AspNetCore.Mvc;
using MentalHealthSupportAPI.DTOs;
using MentalHealthSupportAPI.Services;

namespace MentalHealthSupportAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
        => _authService = authService;

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        // General fejlbesked - afslører ikke om brugernavnet eksisterer (Info Disclosure)
        if (result == null)
            return BadRequest(new { message = "Registration failed." });

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        if (result == null)
            return Unauthorized(new { message = "Invalid credentials." });

        return Ok(result);
    }
}