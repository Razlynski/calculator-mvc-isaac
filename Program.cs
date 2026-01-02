builder.Services.AddDbContext<CalculatorDbContext>(options =>
    options.UseSqlite("Data Source=calculator.db"));
