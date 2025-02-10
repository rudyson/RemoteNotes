using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FPECS.ISTK.Database;

public static class RegisterLayerExtension
{
    public static IServiceCollection UseDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options
                .UseNpgsql(configuration.GetConnectionString("MasterDatabase"))
                .UseSnakeCaseNamingConvention());

        return services;
    }
}
