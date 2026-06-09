using Microsoft.EntityFrameworkCore;
using ProyectoFinal.Data;
using ProyectoFinal.Models;

namespace ProyectoFinal.Services
{
    public class CryptoCurrencyService : ICryptoCurrencyService
    {
        private readonly ApplicationDbContext _context;

        public CryptoCurrencyService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CryptoCurrency>> GetAllAsync()
        {
            return await _context.CryptoCurrencies.ToListAsync();
        }

        public async Task<CryptoCurrency> GetByIdAsync(int id)
        {
            return await _context.CryptoCurrencies.FindAsync(id);
        }

        public async Task<CryptoCurrency> CreateAsync(CryptoCurrency cryptocurrency)
        {
            _context.CryptoCurrencies.Add(cryptocurrency);
            await _context.SaveChangesAsync();
            return cryptocurrency;
        }

        public async Task<CryptoCurrency> UpdateAsync(int id, CryptoCurrency cryptocurrency)
        {
            var existing = await _context.CryptoCurrencies.FindAsync(id);
            if (existing == null)
                return null;

            existing.Name = cryptocurrency.Name;
            existing.Code = cryptocurrency.Code;
            existing.CurrentPrice = cryptocurrency.CurrentPrice;
            existing.LastUpdated = DateTime.Now;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var crypto = await _context.CryptoCurrencies.FindAsync(id);
            if (crypto == null)
                return false;

            _context.CryptoCurrencies.Remove(crypto);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}