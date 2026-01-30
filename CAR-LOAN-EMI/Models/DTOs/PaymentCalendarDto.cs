namespace CAR_LOAN_EMI.Models.DTOs
{
    public class PaymentCalendarDto
    {
        public int EmiNumber { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsPaid { get; set; }
        public bool IsOverdue { get; set; }
        public int DaysUntilDue { get; set; }
        public decimal Principal { get; set; }
        public decimal Interest { get; set; }
    }
}
