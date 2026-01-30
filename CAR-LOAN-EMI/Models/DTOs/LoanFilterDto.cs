using CAR_LOAN_EMI.Models.Enums;

namespace CAR_LOAN_EMI.Models.DTOs
{
    public class LoanFilterDto
    {
        public LoanStatus? Status { get; set; }
        public CarType? CarType { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
