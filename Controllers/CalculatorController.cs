using Microsoft.ASPNETCore.Mvc;
using CalculatorMVC.Models;
using CalculatorMVC.Services;

namespace CalculatorMVC.Controllers
{
    public class CalculatorController : Controller
    {
        private static Calculator _state = new();
        private static CalculatorService _service = new();

        [HttpGet]
        public IActionResult Index()
        {
            return View(_state);
        }

        [HttpPost]
        public IActionResult RefreshUiStateByPress(string action, int? digit)
        {
            _service.HandleInput(_state, action, digit);
            return RedirectToAction("Index");
        }

         public void HandleInput(Calculator state, string action, int? digit)
        {
            if(digit.HasValue)
            {
                InputDigit(state, digit.Value);
                return;
            }

             switch (action)
                {
                    case "=":
                        CalculateResult(state);
                        break;
                    case "AC":
                        Clear(state);
                        break;
                    case "%":
                        state.CurrentValue /= 100;
                        break;
                    case "+":
                    case "-":
                    case "*":
                    case "/":
                        SetOperation(state, action);
                        break;
                }
        }

    }
}