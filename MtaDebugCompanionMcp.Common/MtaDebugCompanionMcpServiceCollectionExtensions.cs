using Microsoft.Extensions.DependencyInjection;
using MtaDebugCompanionMcp.Common.Clients;

namespace MtaDebugCompanionMcp.Common;

public static class MtaDebugCompanionMcpServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Adds the services required for the MTA debug companion MCP.
        /// </summary>
        /// <returns></returns>
        public IServiceCollection AddMtaDebugCompanionMcpServices(string? baseAddress, string? apiKey)
        {
            services.AddHttpClient<MtaServerDebugClient>(x =>
            {
                x.BaseAddress = new Uri(baseAddress ?? "http://localhost:22005");
                x.DefaultRequestHeaders.Add("api-key", apiKey ?? "default");
            });

            return services;
        }
    }
}