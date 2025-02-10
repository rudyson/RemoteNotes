using FPECS.ISTK.Business.Services;
using FPECS.ISTK.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FPECS.ISTK.Business;

public static class RegisterLayerExtension
{
    public static IServiceCollection UseBusinessLogicLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.UseDataAccessLayer(configuration);

        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
