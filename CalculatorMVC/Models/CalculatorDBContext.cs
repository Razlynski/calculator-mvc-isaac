// ============================================================================
// CalculatorDbContext.cs - The Database Context
// ============================================================================
// This class is the "bridge" between your C# code and the actual database.
// Entity Framework Core uses this to know what tables exist and how to 
// connect to the database.

using Microsoft.EntityFrameworkCore;

// Namespace matches the folder structure: CalculatorMVC/Models/
namespace CalculatorMVC.Models
{
    /// <summary>
    /// The DbContext is Entity Framework's main class for database operations.
    /// Think of it as a "session" with the database.
    /// </summary>
    public class CalculatorDbContext : DbContext
    {
        // ====================================================================
        // DbSet<T> - Represents a Table in the Database
        // ====================================================================
        // Each DbSet<T> property represents a table in your database.
        // The generic type <T> is the class that represents one row.
        // 
        // This line says: "There's a table called 'Histories' where each 
        // row is a CalculationHistory object"
        //
        // Entity Framework will automatically:
        // - Create a table named "Histories" 
        // - Create columns for each property in CalculationHistory
        // - Handle all the SQL for you (SELECT, INSERT, UPDATE, DELETE)
        public DbSet<CalculationHistory> Histories { get; set; }

        // ====================================================================
        // Constructor - How to Create this DbContext
        // ====================================================================
        // DbContextOptions contains configuration like:
        // - Which database to use (SQLite, SQL Server, etc.)
        // - Connection string (where is the database file?)
        //
        // We pass these options to the base DbContext class using ": base(options)"
        // This is called "constructor chaining" - we call the parent's constructor.
        //
        // The <CalculatorDbContext> part ensures type safety - the options
        // are specifically for THIS DbContext, not a different one.
        public CalculatorDbContext(DbContextOptions<CalculatorDbContext> options)
            : base(options) 
        { 
            // Empty body - the base constructor does all the work
        }
    }
}