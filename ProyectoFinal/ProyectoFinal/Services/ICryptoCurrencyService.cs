using ProyectoFinal.Models;


namespace ProyectoFinal.Services
{
    public interface ICryptoCurrencyService
    {
        Task<List<CryptoCurrency>> GetAllAsync();
        Task<CryptoCurrency> GetByIdAsync(int id);
        Task<CryptoCurrency> CreateAsync(CryptoCurrency cryptocurrency);
        Task<CryptoCurrency> UpdateAsync(int id, CryptoCurrency cryptocurrency);
        Task<bool> DeleteAsync(int id);
    }
}