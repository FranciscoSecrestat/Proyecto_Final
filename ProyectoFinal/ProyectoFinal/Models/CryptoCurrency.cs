namespace ProyectoFinal.Models
{
    public class CryptoCurrency
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public decimal CurrentPrice { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class Transaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string CryptoCode { get; set; }
        public string Action { get; set; }
        public decimal CryptoAmount { get; set; }
        public decimal Money { get; set; }
        public DateTime TransactionDate { get; set; }
    }

    public class TransactionRequest
    {
        public string CryptoCode { get; set; }
        public string Action { get; set; }
        public decimal CryptoAmount { get; set; }
        public DateTime DateTime { get; set; }
    }
}
