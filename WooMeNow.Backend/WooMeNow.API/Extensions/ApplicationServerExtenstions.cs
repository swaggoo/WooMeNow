using Microsoft.EntityFrameworkCore;
using WooMeNow.API.Data;
using WooMeNow.API.Data.Repository;
using WooMeNow.API.Helpers;
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
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
        services.AddScoped<IPhotoService, PhotoService>();

        return services;
    }
}
