using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoFinal.Models;
using ProyectoFinal.Services;


namespace ProyectoFinal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CryptoCurrenciesController : ControllerBase
    {
        private readonly ICryptoCurrencyService _service;

        public CryptoCurrenciesController(ICryptoCurrencyService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CryptoCurrency>>> GetAll()
        {
            var cryptocurrencies = await _service.GetAllAsync();
            return Ok(cryptocurrencies);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CryptoCurrency>> GetById(int id)
        {
            var crypto = await _service.GetByIdAsync(id);
            if (crypto == null)
                return NotFound();
            return Ok(crypto);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<CryptoCurrency>> Create([FromBody] CryptoCurrency cryptocurrency)
        {
            cryptocurrency.LastUpdated = DateTime.Now;
            var result = await _service.CreateAsync(cryptocurrency);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(int id, [FromBody] CryptoCurrency cryptocurrency)
        {
            var result = await _service.UpdateAsync(id, cryptocurrency);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }

        [HttpGet("prices")]
        public async Task<IActionResult> GetPrices([FromServices] CriptoYaService criptoYaService)
        {
            var cryptos = await _service.GetAllAsync();
            var result = new List<object>();

            foreach (var crypto in cryptos)
            {
                try
                {
                    var price = await criptoYaService.GetPriceAsync(crypto.Code, "purchase");
                    result.Add(new
                    {
                        crypto.Id,
                        crypto.Name,
                        crypto.Code,
                        CurrentPrice = price
                    });
                }
                catch
                {
                    result.Add(new
                    {
                        crypto.Id,
                        crypto.Name,
                        crypto.Code,
                        CurrentPrice = 0
                    });
                }
            }

            return Ok(result);
        }
    }
}