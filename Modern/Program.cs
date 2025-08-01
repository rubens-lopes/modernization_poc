var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
builder.Services.AddControllersWithViews();
var app = builder.Build();

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

    context.Response.Headers.Remove("x-powered-by");

    await next();
});

app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute( name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapReverseProxy();
app.Run();