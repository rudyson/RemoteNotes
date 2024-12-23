using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FPECS.ISTK.Database;

public static class RegisterLayerExtension
{
    public static IServiceCollection RegisterBusinessLayer(this IServiceCollection services, IConfiguration configuration)
    {
        return services;
    }
}
