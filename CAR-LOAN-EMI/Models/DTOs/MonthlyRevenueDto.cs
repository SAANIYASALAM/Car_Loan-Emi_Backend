namespace CAR_LOAN_EMI.Models.DTOs
{
    public class MonthlyRevenueDto
    {
        public List<MonthlyRevenueItemDto> Data { get; set; } = new List<MonthlyRevenueItemDto>();
    }

    public class MonthlyRevenueItemDto
    {
        public string Month { get; set; } = string.Empty;
        public decimal TotalDisbursed { get; set; }
        public List<CarTypeBreakdownDto> CarTypeBreakdown { get; set; } = new List<CarTypeBreakdownDto>();
    }

    public class CarTypeBreakdownDto
    {
        public string CarType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int Count { get; set; }
    }
}
