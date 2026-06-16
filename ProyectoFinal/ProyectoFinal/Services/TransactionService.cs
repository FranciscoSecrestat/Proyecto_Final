using Microsoft.EntityFrameworkCore;
using ProyectoFinal.Data;
using ProyectoFinal.Models;

namespace ProyectoFinal.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ApplicationDbContext _context;
        private readonly CriptoYaService _criptoYaService;

        public TransactionService(ApplicationDbContext context, CriptoYaService criptoYaService)
        {
            _context = context;
            _criptoYaService = criptoYaService;
        }

        public async Task<List<Transaction>> GetByUserAsync(int userId)
        {
            return await _context.Transactions
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }
        private async Task<decimal> GetArsBalanceAsync(int userId)
        {
            var transactions = await _context.Transactions
                .Where(t => t.UserId == userId)
                .ToListAsync();

            decimal balance = 0;
            foreach (var t in transactions)
            {
                if (t.Action == "sale") balance += t.Money;
                else if (t.Action == "purchase") balance -= t.Money;
            }
            return balance;
        }

        public async Task<Transaction> GetByIdAsync(int id)
        {
            return await _context.Transactions.FindAsync(id);
        }

        public async Task<Transaction> CreateAsync(TransactionRequest request, int userId)
        {
            if (request.CryptoAmount <= 0)
                throw new Exception("La cantidad debe ser mayor a 0");

            var price = await _criptoYaService.GetPriceAsync(request.CryptoCode, request.Action);
            var money = price * request.CryptoAmount;

            var user = await _context.Users.FindAsync(userId);

            if (request.Action == "purchase")
            {
                if (money > user.Balance)
                    throw new Exception($"Saldo insuficiente. Tu saldo es $ {user.Balance:N2} y la compra cuesta $ {money:N2}");

                user.Balance -= money;
            }

            if (request.Action == "sale")
            {
                var cryptoBalance = await GetCryptoBalanceAsync(userId, request.CryptoCode);
                if (request.CryptoAmount > cryptoBalance)
                    throw new Exception($"No tenés suficiente {request.CryptoCode}. Saldo disponible: {cryptoBalance}");

                user.Balance += money;
            }

            var transaction = new Transaction
            {
                UserId = userId,
                CryptoCode = request.CryptoCode.ToLower(),
                Action = request.Action,
                CryptoAmount = request.CryptoAmount,
                Money = money,
                TransactionDate = request.DateTime
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<List<Transaction>> GetAllAsync()
        {
            return await _context.Transactions
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
                return false;

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Transaction> UpdateAsync(int id, TransactionRequest request)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
                return null;

            transaction.CryptoCode = request.CryptoCode;
            transaction.Action = request.Action;
            transaction.CryptoAmount = request.CryptoAmount;
            transaction.TransactionDate = request.DateTime;

            await _context.SaveChangesAsync();
            return transaction;
        }

      
        private async Task<decimal> GetCryptoBalanceAsync(int userId, string cryptoCode)
        {
            var transactions = await _context.Transactions
                .Where(t => t.UserId == userId && t.CryptoCode == cryptoCode.ToLower())
                .ToListAsync();

            decimal balance = 0;
            foreach (var t in transactions)
            {
                if (t.Action == "purchase")
                    balance += t.CryptoAmount;
                else if (t.Action == "sale")
                    balance -= t.CryptoAmount;
            }
            return balance;
        }
        
    }
}