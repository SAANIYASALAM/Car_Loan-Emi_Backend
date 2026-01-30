namespace CAR_LOAN_EMI.Models.DTOs
{
    public class LoanRuleDto
    {
        public decimal BaseRate { get; set; }
        public decimal MinDownPaymentPct { get; set; }
        public string? RiskFactor { get; set; }
        public string? Discount { get; set; }
    }
}
