// ============================================================================
// Program.cs - Application Entry Point & Configuration
// ============================================================================
// This is where your ASP.NET Core application starts.
// It configures all the services and middleware the app needs.
//
// The code runs TOP TO BOTTOM when the app starts:
// 1. Create a builder (configuration container)
// 2. Register services (things controllers might need)
// 3. Build the app
// 4. Configure middleware (request processing pipeline)
// 5. Run the app (start listening for requests)

using Microsoft.EntityFrameworkCore;
using CalculatorMVC.Models;
using CalculatorMVC.Services;

// ============================================================================
// STEP 1: Create the Builder
// ============================================================================
// WebApplication.CreateBuilder sets up basic configuration:
// - Reads appsettings.json
// - Sets up logging
// - Prepares dependency injection container
var builder = WebApplication.CreateBuilder(args);

// ============================================================================
// STEP 2: Register Services (Dependency Injection)
// ============================================================================
// "Dependency Injection" (DI) is a design pattern where:
// - Classes declare what they NEED (in constructor)
// - The framework PROVIDES those things automatically
//
// Think of it like ordering at a restaurant:
// - You say "I want a burger" (declare dependency)
// - Kitchen makes it and brings it to you (framework provides)
// - You don't need to know HOW they made it

// ------ MVC Controllers & Views ------
// Adds support for controllers and Razor views
builder.Services.AddControllersWithViews();

// ------ Session Support ------
// Session stores data on the server for each user.
// We use it to store calculator state per window.
//
// AddDistributedMemoryCache: Where to store session data (in-memory)
// AddSession: Enables session middleware
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    // How long before session expires from inactivity
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    
    // Cookie settings for security
    options.Cookie.HttpOnly = true;           // JavaScript can't read the cookie
    options.Cookie.IsEssential = true;        // Cookie works even without consent
});

// ------ Database Context (Entity Framework) ------
// AddDbContext registers our database context for DI.
// When a controller asks for CalculatorDbContext, it gets one automatically.
//
// UseSqlite: Use SQLite database engine
// "Data Source=calculator.db": The database file name
builder.Services.AddDbContext<CalculatorDbContext>(options =>
    options.UseSqlite("Data Source=calculator.db"));

// ------ Calculator Service ------
// AddScoped: Creates ONE instance per HTTP request.
// All controllers in the same request share this instance.
//
// Lifetime options:
// - AddSingleton: One instance for entire app lifetime
// - AddScoped: One instance per request (what we use)
// - AddTransient: New instance every time it's requested
builder.Services.AddScoped<CalculatorService>();

// ============================================================================
// STEP 3: Build the Application
// ============================================================================
// Takes all the configuration and creates the actual app object
var app = builder.Build();

// ============================================================================
// STEP 3.5: Ensure Database Exists
// ============================================================================
// This creates the database file and tables if they don't exist.
// EnsureCreated() is simple - for production, use Migrations instead.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CalculatorDbContext>();
    db.Database.EnsureCreated();  // Creates calculator.db if missing
}

// ============================================================================
// STEP 4: Configure Middleware Pipeline
// ============================================================================
// Middleware = code that runs for EVERY request
// Order matters! Requests flow through in order, responses flow back.
//
// Request:  Browser → HTTPS → Routing → Session → Authorization → Controller
// Response: Browser ← HTTPS ← Routing ← Session ← Authorization ← Controller

// Error handling (production only)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();  // HTTP Strict Transport Security
}

// Redirect HTTP to HTTPS
app.UseHttpsRedirection();

// Enable routing (URL → Controller mapping)
app.UseRouting();

// ------ Session Middleware (IMPORTANT!) ------
// Must be AFTER routing but BEFORE authorization
// This is what makes HttpContext.Session work in controllers
app.UseSession();

// Authorization (checking permissions)
app.UseAuthorization();

// Serve static files (CSS, JS, images from wwwroot/)
app.MapStaticAssets();

// ------ Route Configuration ------
// Maps URLs to Controller/Action
// Pattern: {controller}/{action}/{id?}
// Example: /Calculator/Press/5 → CalculatorController.Press(id=5)
//
// Default: controller=Home, action=Index
// So "/" goes to HomeController.Index()
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// ============================================================================
// STEP 5: Run the Application
// ============================================================================
// Starts the web server and begins listening for requests.
// This line blocks - app runs until you stop it (Ctrl+C)
app.Run();
