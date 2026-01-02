using CalculatorMVC.Models;

namespace CalculatorMVC.Services
{
    public class CalculatorService
    {
        public void InputDigit(CalculatorState state, int digit){
            if (state.IsNewEntry)
            {
                state.CurrentValue = digit;
                state.IsNewEntry = false;
            }
            else
            {
                state.CurrentValue = state.CurrentValue * 10 + digit;
            }
        }

        public void SetOperation(CalculatorState state, string operations)
        {
            state.StoredValue = state.CurrentValue;
            state.Operation = operations;
            state.IsNewEntry = true;
        }

        public void CalculateResult(CalculatorState state)
        {
            switch (state.Operation)
            {
                case "+":
                    state.CurrentValue = state.StoredValue + state.CurrentValue;
                    break;
                case "-":
                    state.CurrentValue = state.StoredValue - state.CurrentValue;
                    break;
                case "*":
                    state.CurrentValue = state.StoredValue * state.CurrentValue;
                    break;
                case "/":
                    if (state.CurrentValue != 0)
                    {
                        state.CurrentValue = state.StoredValue / state.CurrentValue;
                    }
                    else
                    {
                        throw new DivideByZeroException("Cannot divide by zero.");
                    }
                    break;
            }
            state.IsNewEntry = true;
        }

        public void Clear(CalculatorState state)
        {
            state.CurrentValue = 0;
            state.StoredValue = 0;
            state.Operation = "";
            state.IsNewEntry = true;
        }

    }
}