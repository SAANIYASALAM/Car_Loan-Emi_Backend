using CAR_LOAN_EMI.Models.DTOs;
using CAR_LOAN_EMI.Repositories.Interfaces;
using CAR_LOAN_EMI.Services.Interfaces;

namespace CAR_LOAN_EMI.Services.Implementations
{
    public class StatementService : IStatementService
    {
        private readonly IEmiRepository _emiRepository;
        private readonly ILoanRepository _loanRepository;

        public StatementService(IEmiRepository emiRepository, ILoanRepository loanRepository)
        {
            _emiRepository = emiRepository;
            _loanRepository = loanRepository;
        }

        public async Task<ApiResponseDto<StatementDataDto>> GetStatementDataAsync(int userId, int year)
        {
            try
            {
                var transactions = await _emiRepository.GetHistoryByUserIdAsync(userId);
                var yearTransactions = transactions.Where(t => t.PaymentDate.Year == year).ToList();

                var statementData = new StatementDataDto
                {
                    Year = year,
                    TotalTransactions = yearTransactions.Count,
                    TotalPaid = yearTransactions.Sum(t => t.Amount),
                    Transactions = yearTransactions.Select(t => new TransactionDto
                    {
                        Id = t.PaymentId.ToString(),
                        Date = t.PaymentDate,
                        Amount = t.Amount,
                        Type = "EMI Payment",
                        Status = t.Status.ToString(),
                        Method = t.PaymentMethod ?? "N/A"
                    }).OrderByDescending(t => t.Date).ToList()
                };

                return ApiResponseDto<StatementDataDto>.SuccessResponse(statementData, "Statement data retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<StatementDataDto>.ErrorResponse($"Failed to retrieve statement data: {ex.Message}");
            }
        }

        public async Task<byte[]> GeneratePdfStatementAsync(int userId, int year)
        {
            // This is a placeholder. In a real implementation, you would use a PDF library like QuestPDF or iText7
            // For now, return empty byte array
            await Task.CompletedTask;
            return Array.Empty<byte>();
        }
    }
}
