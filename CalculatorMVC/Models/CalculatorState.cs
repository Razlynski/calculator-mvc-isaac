namespace CalculatorMVC.Models
{
    /// <summary>
    /// Represents the current state of a calculator instance.
    /// Each calculator window has its own CalculatorState.
    /// </summary>
    public class CalculatorState
    {
        // ====================================================================
        // PROPERTY 1: WindowId
        // ====================================================================
        // Purpose: Identifies which calculator window this state belongs to.
        // Why we need it: For multi-window support - each browser tab/window
        //                 gets a unique ID so they don't share state.
        // Type: string (we'll use a GUID like "a1b2c3d4-e5f6-...")
        // Default: "" (empty string) - will be assigned when window opens
        public string WindowId { get; set; } = "";

        // ====================================================================
        // PROPERTY 2: CurrentValue
        // ====================================================================
        // Purpose: The number currently displayed on the calculator screen.
        // Why we need it: This is what the user sees and is currently entering.
        // Type: double (decimal number, so we can handle 3.14, not just integers)
        // Default: 0 (calculator starts showing 0)
        public double CurrentValue { get; set; } = 0;

        // ====================================================================
        // PROPERTY 3: StoredValue
        // ====================================================================
        // Purpose: Holds the first number when doing a calculation.
        // Why we need it: When user presses "5 + ", we store 5 here, then
        //                 CurrentValue gets the second number.
        // Example: "5 + 3 =" → StoredValue=5, CurrentValue=3, Result=8
        // Type: double
        // Default: 0
        public double StoredValue { get; set; } = 0;

        // ====================================================================
        // PROPERTY 4: Operation
        // ====================================================================
        // Purpose: Which math operation to perform (+, -, *, /)
        // Why we need it: We need to remember what operation user pressed
        //                 until they press "=" to calculate.
        // Type: string ("+", "-", "*", "/", or "" if none)
        // Default: "" (no operation selected yet)
        public string Operation { get; set; } = "";

        // ====================================================================
        // PROPERTY 5: IsNewEntry
        // ====================================================================
        // Purpose: Flag that tells us if the next digit starts a new number.
        // Why we need it: After pressing "=" or an operator, the next digit
        //                 should REPLACE the display, not append to it.
        // Example: After "5 + 3 = 8", pressing "2" should show "2", not "82"
        // Type: bool (true/false)
        // Default: true (when calculator starts, first digit is a new entry)
        public bool IsNewEntry { get; set; } = true;

        // ====================================================================
        // PROPERTY 6: Expression
        // ====================================================================
        // Purpose: Builds the full expression string for display/history.
        // Why we need it: To show "5 + 3 = 8" in the history panel.
        // Type: string
        // Default: "" (empty when starting)
        public string Expression { get; set; } = "";

        // ====================================================================
        // PROPERTY 7: History
        // ====================================================================
        // Purpose: List of past calculations for this window.
        // Why we need it: To display history panel showing previous calculations.
        // Type: List<CalculationHistory> (a list of CalculationHistory objects)
        // Default: new() creates an empty list
        // Note: This is populated from the database, not stored in session.
        public List<CalculationHistory> History { get; set; } = new();
    }
}
