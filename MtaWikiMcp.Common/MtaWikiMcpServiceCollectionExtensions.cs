using Microsoft.Extensions.DependencyInjection;
using MtaWikiMcp.Common.Clients;
using MtaWikiMcp.Common.Services;
using System.Threading.RateLimiting;

namespace MtaWikiMcp.Common;

public static class MtaWikiMcpServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Adds the services required for the MTA Wiki MCP.
        /// </summary>
        /// <returns></returns>
        public IServiceCollection AddMtaWikiMcpServices()
        {
            services.AddSingleton<GameDataProvider>();

            services.AddHttpClient<WikiClient>(x => x.BaseAddress = new Uri("https://wiki.multitheftauto.com/wiki/"))
                .AddStandardResilienceHandler(options =>
                {
                    options.Retry.MaxRetryAttempts = 4;
                    options.Retry.Delay = TimeSpan.FromSeconds(1);

                    options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(60);
                    options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(15);

                    options.RateLimiter.DefaultRateLimiterOptions = new ConcurrencyLimiterOptions
                    {
                        PermitLimit = 10,
                        QueueLimit = 5
                    };
                });

            return services;
        }
    }
}