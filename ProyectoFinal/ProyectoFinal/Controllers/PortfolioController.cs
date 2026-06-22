using Microsoft.AspNetCore.Mvc;
using ProyectoFinal.Data;
using ProyectoFinal.Services;
using Microsoft.EntityFrameworkCore;

namespace ProyectoFinal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PortfolioController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly CriptoYaService _criptoYaService;

        public PortfolioController(ApplicationDbContext context, CriptoYaService criptoYaService)
        {
            _context = context;
            _criptoYaService = criptoYaService;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetPortfolio(int userId)
        {
            var transactions = await _context.Transactions
                .Where(t => t.UserId == userId)
                .ToListAsync();

            var cryptoCodes = transactions.Select(t => t.CryptoCode).Distinct();
            var portfolio = new List<object>();
            decimal total = 0;

            foreach (var code in cryptoCodes)
            {
                decimal balance = 0;
                foreach (var t in transactions.Where(t => t.CryptoCode == code))
                {
                    if (t.Action == "purchase") balance += t.CryptoAmount;
                    else balance -= t.CryptoAmount;
                }

                if (balance <= 0) continue;

                var price = await _criptoYaService.GetPriceAsync(code, "sale");
                var value = balance * price;
                total += value;

                portfolio.Add(new
                {
                    CryptoCode = code.ToUpper(),
                    Amount = balance,
                    CurrentPrice = price,
                    TotalValue = value
                });
            }

            return Ok(new { portfolio, total });
        }
    }
}
