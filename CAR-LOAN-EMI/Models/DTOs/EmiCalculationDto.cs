namespace CAR_LOAN_EMI.Models.DTOs
{
    public class EmiCalculationDto
    {
        public decimal Principal { get; set; }
        public decimal Rate { get; set; }
        public int Tenure { get; set; }
    }
}
