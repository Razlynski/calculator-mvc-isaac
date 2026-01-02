using Microsoft.EntityFrameworkCore;

namespace CalculatorMVC.Data
{
    public class CalculatorDbContext : DbContext
    {
        public DbSet<CalculationHistory> Histories { get; set; }

        public CalculatorDbContext(DbContextOptions options)
            : base(options) { }
    }
}