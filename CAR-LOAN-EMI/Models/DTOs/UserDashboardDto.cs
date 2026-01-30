namespace CAR_LOAN_EMI.Models.DTOs
{
    public class UserDashboardDto
    {
        public decimal PreApprovedLimit { get; set; }
        public int CreditScore { get; set; }
        public int LoanHealthScore { get; set; }
        public int ActiveLoans { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalRemaining { get; set; }
        public List<PersonalizedOfferDto> PersonalizedOffers { get; set; } = new List<PersonalizedOfferDto>();
    }

    public class PersonalizedOfferDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string OfferType { get; set; } = string.Empty;
        public DateTime ValidUntil { get; set; }
    }
}
