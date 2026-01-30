using CAR_LOAN_EMI.Models.DTOs;
using CAR_LOAN_EMI.Models.Enums;

namespace CAR_LOAN_EMI.Helpers
{
    public class LoanRulesEngine
    {
        public LoanRuleDto GetLoanRules(CarType carType)
        {
            return carType switch
            {
                CarType.Hatchback => new LoanRuleDto
                {
                    BaseRate = 9.0m,
                    MinDownPaymentPct = 10m,
                    RiskFactor = "Low - Best for First Time Buyers",
                    Discount = null
                },
                CarType.ElectricVehicle or CarType.Hybrid => new LoanRuleDto
                {
                    BaseRate = 8.5m,
                    MinDownPaymentPct = 10m,
                    RiskFactor = "Low",
                    Discount = "Green Loan (1% Off applied)"
                },
                CarType.MidSizeSUV or CarType.FullSizeSUV or CarType.LuxurySUV or CarType.LuxurySedan => new LoanRuleDto
                {
                    BaseRate = 9.5m,
                    MinDownPaymentPct = 20m,
                    RiskFactor = "Moderate - Higher Down Payment Required",
                    Discount = null
                },
                CarType.UsedCar => new LoanRuleDto
                {
                    BaseRate = 11.5m,
                    MinDownPaymentPct = 10m,
                    RiskFactor = "High - Short Tenure Recommended",
                    Discount = null
                },
                CarType.Commercial => new LoanRuleDto
                {
                    BaseRate = 12.0m,
                    MinDownPaymentPct = 25m,
                    RiskFactor = "High",
                    Discount = null
                },
                _ => new LoanRuleDto
                {
                    BaseRate = 9.5m,
                    MinDownPaymentPct = 10m,
                    RiskFactor = "Low",
                    Discount = null
                }
            };
        }
    }
}
