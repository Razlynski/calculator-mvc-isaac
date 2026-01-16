// ============================================================================
// CalculatorService.cs - The Business Logic Layer
// ============================================================================
// This class contains ALL the calculator math logic.
// The Controller will call these methods, but won't do math itself.
//
// WHY SEPARATE?
// 1. Testability: You can unit test math without a web server
// 2. Reusability: Same logic could power a mobile app or API
// 3. Single Responsibility: Each class does ONE thing well

using CalculatorMVC.Models;

namespace CalculatorMVC.Services
{
    /// <summary>
    /// Handles all calculator operations: digits, operations, and calculations.
    /// </summary>
    public class CalculatorService
    {
        // ====================================================================
        // METHOD 1: InputDigit
        // ====================================================================
        // Called when user presses a number button (0-9)
        //
        // Parameters:
        //   state - The current calculator state (passed by reference-like behavior)
        //   digit - The number pressed (0-9)
        //
        // Logic:
        //   If IsNewEntry is true  → Replace display with this digit
        //   If IsNewEntry is false → Append digit to current number
        //
        // Example:
        //   Display shows "0", IsNewEntry=true, press "5" → Display shows "5"
        //   Display shows "5", IsNewEntry=false, press "3" → Display shows "53"
        public void InputDigit(CalculatorState state, int digit)
        {
            if (state.IsNewEntry)
            {
                // Starting a new number - replace whatever was there
                state.CurrentValue = digit;
                state.IsNewEntry = false;
                
                // Build expression string (for history)
                // If there's already an operation, add to expression
                // Otherwise, start fresh
                if (string.IsNullOrEmpty(state.Operation))
                {
                    state.Expression = digit.ToString();
                }
                else
                {
                    state.Expression += digit.ToString();
                }
            }
            else
            {
                // Appending to existing number
                // Math trick: "53" = 5 * 10 + 3
                state.CurrentValue = state.CurrentValue * 10 + digit;
                state.Expression += digit.ToString();
            }
        }

        // ====================================================================
        // METHOD 2: InputDecimal
        // ====================================================================
        // Called when user presses the "." button
        // NOTE: For simplicity, our basic version uses integers.
        //       A full implementation would track decimal places.
        //       We'll skip this for now but the method is here for future use.
        public void InputDecimal(CalculatorState state)
        {
            // TODO: Implement decimal support
            // Would need to track: hasDecimalPoint, decimalPlaces
        }

        // ====================================================================
        // METHOD 3: SetOperation
        // ====================================================================
        // Called when user presses an operator (+, -, *, /)
        //
        // What happens:
        // 1. Store the current number (it becomes the "left" operand)
        // 2. Remember which operation was pressed
        // 3. Get ready for the next number (set IsNewEntry = true)
        //
        // Example: User presses "5", then "+"
        //   → StoredValue = 5
        //   → Operation = "+"
        //   → IsNewEntry = true (next digit starts fresh)
        public void SetOperation(CalculatorState state, string operation)
        {
            // If there's already a pending operation, calculate it first
            // This handles: 5 + 3 + 2 (calculates 5+3 before adding 2)
            if (!string.IsNullOrEmpty(state.Operation) && !state.IsNewEntry)
            {
                CalculateResult(state);
            }
            
            state.StoredValue = state.CurrentValue;
            state.Operation = operation;
            state.IsNewEntry = true;
            
            // Add operation to expression (with spaces for readability)
            state.Expression += " " + operation + " ";
        }

        // ====================================================================
        // METHOD 4: CalculateResult
        // ====================================================================
        // Called when user presses "="
        //
        // Performs the stored operation:
        //   Result = StoredValue [operation] CurrentValue
        //
        // Example: StoredValue=5, Operation="+", CurrentValue=3
        //          Result = 5 + 3 = 8
        public void CalculateResult(CalculatorState state)
        {
            // Only calculate if we have an operation pending
            if (string.IsNullOrEmpty(state.Operation))
            {
                return; // Nothing to calculate
            }

            double result = 0;
            
            // Switch statement - like multiple if/else but cleaner
            // Checks which operation to perform
            switch (state.Operation)
            {
                case "+":
                    result = state.StoredValue + state.CurrentValue;
                    break;  // Exit the switch (don't fall through)
                    
                case "-":
                    result = state.StoredValue - state.CurrentValue;
                    break;
                    
                case "*":
                    result = state.StoredValue * state.CurrentValue;
                    break;
                    
                case "/":
                    // Division by zero check - prevent crash!
                    if (state.CurrentValue != 0)
                    {
                        result = state.StoredValue / state.CurrentValue;
                    }
                    else
                    {
                        // Could show "Error" - for now just return 0
                        result = 0;
                    }
                    break;
            }

            // Update state with result
            state.CurrentValue = result;
            state.Operation = "";       // Clear the operation
            state.IsNewEntry = true;    // Next digit starts fresh
            
            // Expression is complete now (will be saved to history)
        }

        // ====================================================================
        // METHOD 5: Clear
        // ====================================================================
        // Called when user presses "AC" (All Clear)
        //
        // Resets everything back to initial state
        public void Clear(CalculatorState state)
        {
            state.CurrentValue = 0;
            state.StoredValue = 0;
            state.Operation = "";
            state.IsNewEntry = true;
            state.Expression = "";
        }

        // ====================================================================
        // METHOD 6: Percent
        // ====================================================================
        // Called when user presses "%"
        //
        // Converts current value to percentage (divides by 100)
        // Example: 50% → 0.5
        public void Percent(CalculatorState state)
        {
            state.CurrentValue = state.CurrentValue / 100;
        }

        // ====================================================================
        // METHOD 7: ToggleSign
        // ====================================================================
        // Called when user presses "+/-"
        //
        // Flips the sign: positive → negative, negative → positive
        public void ToggleSign(CalculatorState state)
        {
            state.CurrentValue = -state.CurrentValue;
        }
    }
}
