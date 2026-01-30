using Microsoft.EntityFrameworkCore;
using CAR_LOAN_EMI.Models.Entities;
using CAR_LOAN_EMI.Models.Enums;

namespace CAR_LOAN_EMI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Loan> Loans { get; set; } = null!;
        public DbSet<EmiPayment> EmiPayments { get; set; } = null!;
        public DbSet<LoanRule> LoanRules { get; set; } = null!;
        public DbSet<KycDocument> KycDocuments { get; set; } = null!;
        public DbSet<ContactMessage> ContactMessages { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User-Loan relationship
            modelBuilder.Entity<Loan>()
                .HasOne(l => l.User)
                .WithMany(u => u.Loans)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Loan-EmiPayment relationship
            modelBuilder.Entity<EmiPayment>()
                .HasOne(e => e.Loan)
                .WithMany(l => l.EmiPayments)
                .HasForeignKey(e => e.LoanId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure User-KycDocument relationship
            modelBuilder.Entity<KycDocument>()
                .HasOne(k => k.User)
                .WithMany(u => u.KycDocuments)
                .HasForeignKey(k => k.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure User-ContactMessage relationship
            modelBuilder.Entity<ContactMessage>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Seed LoanRules data
            modelBuilder.Entity<LoanRule>().HasData(
                new LoanRule
                {
                    RuleId = 1,
                    CarType = CarType.Hatchback,
                    BaseRate = 9.0m,
                    MinDownPaymentPercent = 10m,
                    RiskFactor = "Low",
                    IsActive = true
                },
                new LoanRule
                {
                    RuleId = 2,
                    CarType = CarType.ElectricVehicle,
                    BaseRate = 8.5m,
                    MinDownPaymentPercent = 10m,
                    Discount = "Green Loan",
                    IsActive = true
                },
                new LoanRule
                {
                    RuleId = 3,
                    CarType = CarType.LuxurySUV,
                    BaseRate = 9.5m,
                    MinDownPaymentPercent = 20m,
                    RiskFactor = "Moderate",
                    IsActive = true
                },
                new LoanRule
                {
                    RuleId = 4,
                    CarType = CarType.UsedCar,
                    BaseRate = 11.5m,
                    MinDownPaymentPercent = 10m,
                    RiskFactor = "High",
                    IsActive = true
                }
            );
        }
    }
}
