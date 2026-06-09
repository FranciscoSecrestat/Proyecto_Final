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
        public async Task<ActionResult<CryptoCurrency>> Create([FromBody] CryptoCurrency cryptocurrency)
        {
            cryptocurrency.LastUpdated = DateTime.Now;
            var result = await _service.CreateAsync(cryptocurrency);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CryptoCurrency cryptocurrency)
        {
            var result = await _service.UpdateAsync(id, cryptocurrency);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }
    }
}