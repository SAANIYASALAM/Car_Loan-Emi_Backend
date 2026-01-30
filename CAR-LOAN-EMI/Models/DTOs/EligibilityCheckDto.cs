namespace CAR_LOAN_EMI.Models.DTOs
{
    public class EligibilityCheckDto
    {
        public bool Eligible { get; set; }
        public string Reason { get; set; } = string.Empty;
        public decimal EstimatedEmi { get; set; }
        public decimal InterestRate { get; set; }
    }
}
