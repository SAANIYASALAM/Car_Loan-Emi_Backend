namespace CAR_LOAN_EMI.Models.DTOs
{
    public class DashboardStatsDto
    {
        public decimal TotalLoansDisbursed { get; set; }
        public int ActiveUsers { get; set; }
        public int PendingApprovals { get; set; }
        public int TotalLoans { get; set; }
        public decimal AverageInterestRate { get; set; }
        public decimal ThisMonthDisbursals { get; set; }
    }
}
