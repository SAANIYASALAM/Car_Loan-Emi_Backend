namespace CAR_LOAN_EMI.Models.DTOs
{
    public class CarTypeDistributionDto
    {
        public List<CarTypeDistributionItemDto> Distribution { get; set; } = new List<CarTypeDistributionItemDto>();
    }

    public class CarTypeDistributionItemDto
    {
        public string CarType { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Percentage { get; set; }
    }
}
