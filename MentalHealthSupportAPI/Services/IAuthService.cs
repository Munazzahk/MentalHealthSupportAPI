using MentalHealthSupportAPI.DTOs;

namespace MentalHealthSupportAPI.Services
{
    public interface IAuthService
    {
        Task<AuthResponse?> RegisterAsync(RegisterRequest request);
        Task<AuthResponse?> LoginAsync(LoginRequest request);
    }
}
