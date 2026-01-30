namespace CAR_LOAN_EMI.Models.DTOs
{
    public class StatementDataDto
    {
        public int Year { get; set; }
        public int TotalTransactions { get; set; }
        public decimal TotalPaid { get; set; }
        public List<TransactionDto> Transactions { get; set; } = new List<TransactionDto>();
    }

    public class TransactionDto
    {
        public string Id { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
    }
}
