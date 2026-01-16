// ============================================================================
// CalculatorController.cs - The Traffic Cop
// ============================================================================
// Controllers in MVC:
// 1. Receive HTTP requests from the browser
// 2. Process them (using Services for business logic)
// 3. Return a response (usually a View with data)
//
// This controller handles all calculator-related requests.

using Microsoft.AspNetCore.Mvc;
using CalculatorMVC.Models;
using CalculatorMVC.Services;
using System.Text.Json;

namespace CalculatorMVC.Controllers
{
    // ========================================================================
    // The Controller Class
    // ========================================================================
    // "Controller" suffix is a convention - ASP.NET recognizes it automatically
    // ": Controller" means we inherit from the base Controller class,
    // which gives us methods like View(), RedirectToAction(), etc.
    public class CalculatorController : Controller
    {
        // ====================================================================
        // DEPENDENCIES (Things this controller needs)
        // ====================================================================
        
        // The service that does actual calculations
        // "readonly" means it can only be set in constructor, not changed later
        private readonly CalculatorService _service;
        
        // The database context for saving history
        private readonly CalculatorDbContext _db;

        // ====================================================================
        // CONSTRUCTOR - Called when controller is created
        // ====================================================================
        // ASP.NET's "Dependency Injection" automatically provides these
        // parameters when it creates the controller.
        //
        // Why Dependency Injection?
        // - Makes testing easier (can inject mock services)
        // - Controller doesn't need to know HOW to create dependencies
        // - Centralized configuration in Program.cs
        public CalculatorController(CalculatorService service, CalculatorDbContext db)
        {
            _service = service;
            _db = db;
        }

        // ====================================================================
        // HELPER: Get/Set Calculator State from Session
        // ====================================================================
        // Session = temporary storage on the server for each user
        // We store the calculator state here, keyed by windowId
        //
        // Session stores data as strings, so we use JSON to convert:
        // Object → JSON string (serialize) → Store in session
        // Session → JSON string → Object (deserialize)
        
        private string GetSessionKey(string windowId) => $"calc_{windowId}";
        
        private CalculatorState GetState(string windowId)
        {
            var key = GetSessionKey(windowId);
            var json = HttpContext.Session.GetString(key);
            
            if (string.IsNullOrEmpty(json))
            {
                // No state exists - create a new one
                var newState = new CalculatorState { WindowId = windowId };
                SaveState(newState);
                return newState;
            }
            
            // Deserialize JSON back to object
            // The "!" tells C# "I know this won't be null, trust me"
            return JsonSerializer.Deserialize<CalculatorState>(json)!;
        }
        
        private void SaveState(CalculatorState state)
        {
            var key = GetSessionKey(state.WindowId);
            var json = JsonSerializer.Serialize(state);
            HttpContext.Session.SetString(key, json);
        }

        // ====================================================================
        // ACTION: Index (GET /Calculator or GET /Calculator/Index)
        // ====================================================================
        // [HttpGet] - This action responds to GET requests (page loads)
        //
        // Parameters:
        //   windowId - Optional. If not provided, generates a new one.
        //              This enables multi-window support.
        //
        // Returns:
        //   The calculator View with the current state
        [HttpGet]
        public IActionResult Index(string? windowId)
        {
            // If no windowId provided, create a new one (new window)
            // Guid.NewGuid() creates a unique ID like "a1b2c3d4-e5f6-..."
            if (string.IsNullOrEmpty(windowId))
            {
                windowId = Guid.NewGuid().ToString();
                // Redirect to include windowId in URL
                // This way, bookmarking/refreshing keeps the same window
                return RedirectToAction("Index", new { windowId });
            }
            
            // Get state for this window
            var state = GetState(windowId);
            
            // Load history from database for this window
            state.History = _db.Histories
                .Where(h => h.WindowId == windowId)    // Filter by window
                .OrderByDescending(h => h.CreatedAt)   // Newest first
                .Take(10)                               // Last 10 only
                .ToList();
            
            // Pass state to the View
            return View(state);
        }

        // ====================================================================
        // ACTION: Press (POST /Calculator/Press)
        // ====================================================================
        // [HttpPost] - This action responds to POST requests (form submissions)
        //
        // Parameters (from form):
        //   windowId - Which calculator window
        //   action   - "AC", "+", "-", "*", "/", "=", "%", "+/-" (or null if digit)
        //   digit    - 0-9 (or null if action)
        //
        // How it works:
        //   - Only ONE of "action" or "digit" will have a value
        //   - We check which one and call the appropriate service method
        [HttpPost]
        public IActionResult Press(string windowId, string? action, int? digit)
        {
            // Get current state for this window
            var state = GetState(windowId);
            
            // ----------------------------------------------------------------
            // Handle DIGIT press (0-9)
            // ----------------------------------------------------------------
            // digit.HasValue checks if it's not null
            if (digit.HasValue)
            {
                _service.InputDigit(state, digit.Value);
                SaveState(state);
                return RedirectToAction("Index", new { windowId });
            }
            
            // ----------------------------------------------------------------
            // Handle ACTION press
            // ----------------------------------------------------------------
            switch (action)
            {
                case "AC":
                    // All Clear - reset calculator
                    _service.Clear(state);
                    break;
                    
                case "%":
                    // Percent - divide by 100
                    _service.Percent(state);
                    break;
                    
                case "+/-":
                    // Toggle sign - positive/negative
                    _service.ToggleSign(state);
                    break;
                    
                case "+":
                case "-":
                case "*":
                case "/":
                    // Arithmetic operations
                    _service.SetOperation(state, action);
                    break;
                    
                case "=":
                    // Calculate result and save to history
                    string expression = state.Expression;
                    _service.CalculateResult(state);
                    
                    // Save to history (only if there was an actual calculation)
                    if (!string.IsNullOrEmpty(expression) && expression.Contains(" "))
                    {
                        var history = new CalculationHistory
                        {
                            WindowId = windowId,
                            Expression = expression,
                            Result = state.CurrentValue,
                            CreatedAt = DateTime.Now
                        };
                        _db.Histories.Add(history);
                        _db.SaveChanges();  // Commit to database
                    }
                    
                    // Clear expression for next calculation
                    state.Expression = "";
                    break;
            }
            
            // Save updated state and redirect back to calculator
            SaveState(state);
            return RedirectToAction("Index", new { windowId });
        }

        // ====================================================================
        // ACTION: NewWindow (GET /Calculator/NewWindow)
        // ====================================================================
        // Opens a new calculator window with a fresh state
        [HttpGet]
        public IActionResult NewWindow()
        {
            // Just redirect to Index without a windowId
            // Index will generate a new one
            return RedirectToAction("Index");
        }

        // ====================================================================
        // ACTION: ClearHistory (POST /Calculator/ClearHistory)
        // ====================================================================
        // Clears all history for this window
        [HttpPost]
        public IActionResult ClearHistory(string windowId)
        {
            // Find all history entries for this window
            var historyItems = _db.Histories
                .Where(h => h.WindowId == windowId)
                .ToList();
            
            // Remove them all
            _db.Histories.RemoveRange(historyItems);
            _db.SaveChanges();
            
            return RedirectToAction("Index", new { windowId });
        }
    }
}
