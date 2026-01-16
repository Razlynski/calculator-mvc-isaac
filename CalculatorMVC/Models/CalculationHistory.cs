// ============================================================================
// CalculationHistory.cs - Entity Model for Database Storage
// ============================================================================
// This class represents ONE ROW in the "Histories" database table.
// Entity Framework maps this class to a table automatically.

namespace CalculatorMVC.Models
{
    /// <summary>
    /// Represents a single calculation that was performed.
    /// Each time you press "=" on the calculator, one of these gets saved.
    /// </summary>
    public class CalculationHistory
    {
        // ====================================================================
        // PROPERTY 1: Id (Primary Key)
        // ====================================================================
        // Every database table needs a PRIMARY KEY - a unique identifier.
        // Entity Framework sees "Id" or "[ClassName]Id" and automatically
        // makes it the primary key with auto-increment.
        //
        // Auto-increment means: first record gets Id=1, next gets Id=2, etc.
        // You never set this yourself - the database assigns it.
        public int Id { get; set; }

        // ====================================================================
        // PROPERTY 2: Expression
        // ====================================================================
        // The math expression that was calculated, like "5 + 3"
        // We store this so the history panel can show what was done.
        public string Expression { get; set; } = "";

        // ====================================================================
        // PROPERTY 3: Result
        // ====================================================================
        // The answer to the calculation, like 8 (from "5 + 3")
        // Using double to support decimal results like 10 / 3 = 3.333...
        public double Result { get; set; }

        // ====================================================================
        // PROPERTY 4: CreatedAt
        // ====================================================================
        // When this calculation was performed.
        // DateTime is C#'s type for dates and times.
        // We use this to sort history (newest first) and show timestamps.
        public DateTime CreatedAt { get; set; }

        // ====================================================================
        // PROPERTY 5: WindowId
        // ====================================================================
        // Which calculator window performed this calculation.
        // This enables multi-window support - each window has its own history.
        // If you open 2 calculator tabs, each sees only its own calculations.
        public string WindowId { get; set; } = "";
    }
}
