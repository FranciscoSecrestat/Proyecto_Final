using ProyectoFinal.Models;

namespace ProyectoFinal.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<User> GetUserByEmailAsync(string email);
        Task<bool> DepositAsync(int userId, decimal amount);
        Task<User> GetUserByIdAsync(int userId);
        Task<List<Deposit>> GetDepositsAsync(int userId);
    }
}
