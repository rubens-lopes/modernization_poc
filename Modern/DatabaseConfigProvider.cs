using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

using ModernizationPoC.Modern.DAL;

using Yarp.ReverseProxy.Configuration;

namespace ModernizationPoC.Modern;

public class DatabaseConfigProvider : IProxyConfigProvider
{
    private volatile IProxyConfig _config;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly object _lock = new();

    public DatabaseConfigProvider(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        _config = LoadConfigAsync().GetAwaiter().GetResult();
    }

    public IProxyConfig GetConfig() => _config;

    public async Task ReloadAsync()
    {
        try
        {
            var newConfig = await LoadConfigAsync();
            
            lock (_lock)
            {
                var oldConfig = _config;
                _config = newConfig;

                // Signal change AFTER swapping to new config
                ((DatabaseProxyConfig)oldConfig).SignalChange();
            }
        }
        catch
        {
            // ignored
        }
    }

    private async Task<IProxyConfig> LoadConfigAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var proxyAbout = await dbContext.Toggles.AnyAsync(t => t.AboutPage == false);

        var routes = new List<RouteConfig>
        {
            new()
            {
                RouteId = "fallbackRoute",
                ClusterId = "fallbackCluster",
                Order = 1,
                Match = new RouteMatch
                {
                    Path = "{**catch-all}",
                },
            },
        };

        if (proxyAbout)
            routes.Add(new RouteConfig
            {
                RouteId = "forceFallback",
                ClusterId = "fallbackCluster",
                Order = 0,
                Match = new RouteMatch { Path = "/Home/About" },
            });

        var clusters = new List<ClusterConfig>
        {
            new()
            {
                ClusterId = "fallbackCluster",
                Destinations = new Dictionary<string, DestinationConfig>
                {
                    ["fallbackApp"] = new()
                    {
                        Address = "http://localhost:5000/",
                    },
                },
            },
        };

        return new DatabaseProxyConfig(routes, clusters);
    }
}

public class DatabaseProxyConfig : IProxyConfig
{
    private readonly CancellationTokenSource _cts = new();

    public DatabaseProxyConfig(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters)
    {
        RevisionId = Guid.NewGuid().ToString();
        Routes = routes;
        Clusters = clusters;
        ChangeToken = new CancellationChangeToken(_cts.Token);
    }

    public string RevisionId { get; }
    public IReadOnlyList<RouteConfig> Routes { get; }
    public IReadOnlyList<ClusterConfig> Clusters { get; }
    public IChangeToken ChangeToken { get; }

    public void SignalChange()
    {
        _cts.Cancel();
    }
}
