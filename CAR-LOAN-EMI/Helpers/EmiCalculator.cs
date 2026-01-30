namespace CAR_LOAN_EMI.Helpers
{
    public static class EmiCalculator
    {
        /// <summary>
        /// Calculate EMI using the formula: EMI = [P x R x (1+R)^N] / [(1+R)^N-1]
        /// Where P = principal, R = monthly rate, N = number of months
        /// </summary>
        public static decimal CalculateEMI(decimal principal, decimal annualRate, int tenureYears)
        {
            if (principal <= 0 || annualRate <= 0 || tenureYears <= 0)
                return 0;

            // Convert annual rate to monthly rate (divide by 12 and by 100)
            decimal monthlyRate = annualRate / 12 / 100;
            
            // Calculate number of months
            int numberOfMonths = tenureYears * 12;

            // Calculate EMI using the formula
            decimal onePlusR = 1 + monthlyRate;
            decimal onePlusRPowerN = (decimal)Math.Pow((double)onePlusR, numberOfMonths);
            
            decimal emi = (principal * monthlyRate * onePlusRPowerN) / (onePlusRPowerN - 1);

            // Round to 2 decimal places
            return Math.Round(emi, 2);
        }
    }
}
