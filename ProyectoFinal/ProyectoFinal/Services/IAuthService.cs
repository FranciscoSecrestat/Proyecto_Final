using ProyectoFinal.Models;

namespace ProyectoFinal.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<User> GetUserByEmailAsync(string email);
    }
}
