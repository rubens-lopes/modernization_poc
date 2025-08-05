using ModernizationPoC.Modern.DAL;

namespace ModernizationPoC.Modern;

public class ModernToggleMiddleware(RequestDelegate next, EndpointDataSource endpointDataSource)
{
    private readonly Endpoint _fallbackRoute = endpointDataSource.Endpoints.First(endpoint => endpoint.DisplayName == "fallbackRoute");
    public async Task Invoke(HttpContext context)
    {
        var path = context.Request.Path.Value ?? "/";
        var dbContext = context.RequestServices.GetRequiredService<ApplicationDbContext>();
        var logger = context.RequestServices.GetRequiredService<ILogger<ModernToggleMiddleware>>();

        try
        {
            if (path.Contains("About") == false)
            {
                await next(context); // Continue through YARP pipeline
                return;
            }

            var shouldUseLegacy = dbContext.Toggles.Any(t => t.AboutPage == false);

            if (shouldUseLegacy)
            {

                context.SetEndpoint(_fallbackRoute);
                logger.LogInformation(context.GetEndpoint()!.ToString());
                await next(context); // Continue through YARP pipeline
                return;
            }
            
            await next(context); // Continue through YARP pipeline

        }
        catch
        {
            await next(context); // Continue on error
        }
    }
}