using Microsoft.EntityFrameworkCore;
using WooMeNow.API.Data;
using WooMeNow.API.Interfaces;
using WooMeNow.API.Services;

namespace WooMeNow.API.Extensions;

public static class ApplicationServerExtenstions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        IConfiguration config)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlite(config.GetConnectionString("DefaultConnection"));
        });
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddScoped<ITokenService, TokenService>();

        return services;
    }
}
