using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using WooMeNow.API.Data;
using WooMeNow.API.Interfaces;
using WooMeNow.API.Services;
using System.Text;
using WooMeNow.API.Extensions;
using WooMeNow.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
app.UseCors(policyBuilder =>
{
    policyBuilder.AllowAnyHeader()
    .AllowAnyMethod()
    .WithOrigins("https://localhost:4200");
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseHttpsRedirection();

app.UseAuthorization();

app.Run();
