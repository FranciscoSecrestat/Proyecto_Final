using ProyectoFinal.Models;

namespace ProyectoFinal.Services
{
    public interface ITransactionService
    {
        Task<List<Transaction>> GetAllAsync();
        Task<List<Transaction>> GetByUserAsync(int userId);
        Task<Transaction> GetByIdAsync(int id);
        Task<Transaction> CreateAsync(TransactionRequest request, int userId);
        Task<Transaction> UpdateAsync(int id, TransactionRequest request);
        Task<bool> DeleteAsync(int id);
    }
}