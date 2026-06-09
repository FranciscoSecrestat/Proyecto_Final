using System.Text.Json;

namespace ProyectoFinal.Services
{
    public class CriptoYaService
    {
        private readonly HttpClient _httpClient;

        public CriptoYaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal> GetPriceAsync(string cryptoCode, string action)
        {
            // action: purchase → usamos "ask", sale → usamos "bid"
            var url = $"https://criptoya.com/api/satoshitango/{cryptoCode}/ars";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonDocument.Parse(json).RootElement;

            if (action == "purchase")
                return data.GetProperty("totalAsk").GetDecimal();
            else
                return data.GetProperty("totalBid").GetDecimal();
        }
    }
}