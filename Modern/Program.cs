using Microsoft.EntityFrameworkCore;
using ModernizationPoC.Modern.DAL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
builder.Services.AddControllersWithViews();

// Set up DataDirectory path
var solutionDir = Directory.GetParent(Directory.GetCurrentDirectory())?.FullName;
string dataDirectory = string.Empty;

if (!string.IsNullOrEmpty(solutionDir))
{
    dataDirectory = Path.Combine(solutionDir, "Legacy", "App_Data");
    
    // Ensure the directory exists
    if (!Directory.Exists(dataDirectory))
    {
        Directory.CreateDirectory(dataDirectory);
    }
    
    AppDomain.CurrentDomain.SetData("DataDirectory", dataDirectory);
}

// Configure Entity Framework with proper connection string
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    
    // Replace DataDirectory placeholder with actual path
    if (!string.IsNullOrEmpty(dataDirectory) && connectionString.Contains("|DataDirectory|"))
    {
        connectionString = connectionString.Replace("|DataDirectory|", dataDirectory);
    }
    
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        // Enable retry on failure for transient errors
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    });
});

var app = builder.Build();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        context.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        // Log the exception (you might want to use proper logging here)
        Console.WriteLine($"Database initialization error: {ex.Message}");
        
        // Try with a simpler connection string for LocalDB
        var fallbackConnectionString = "Server=(LocalDB)\\MSSQLLocalDB;Database=ModernizationPoC;Trusted_Connection=True;MultipleActiveResultSets=true;AttachDbFilename=|DataDirectory|\\ModernizationPoC.mdf";
        
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlServer(fallbackConnectionString.Replace("|DataDirectory|", dataDirectory));
        
        using var fallbackContext = new ApplicationDbContext(optionsBuilder.Options);
        fallbackContext.Database.EnsureCreated();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    if (!context.Response.Headers.ContainsKey("Strict-Transport-Security"))
        context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000; includeSubDomains; preload");
    
    if (!context.Response.Headers.ContainsKey("X-Content-Type-Options"))
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    
    if (!context.Response.Headers.ContainsKey("X-Frame-Options"))
        context.Response.Headers.Append("X-Frame-Options", "DENY");

    await next();
});

app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapReverseProxy();
app.Run();