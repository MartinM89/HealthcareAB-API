using System.Diagnostics.CodeAnalysis;
using System.Text;
using HealthCareAB_v1.Configuration;
using HealthCareAB_v1.Constants;
using HealthCareAB_v1.Repositories.Implementations;
using HealthCareAB_v1.Repositories.Interfaces;
using HealthCareAB_v1.Services.Implementations;
using HealthCareAB_v1.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace HealthCareAB_v1.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor(); // Required for AuthService to check current user
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<ITimeSlotService, TimeSlotService>();
        services.AddScoped<ITimeSlotRepository, TimeSlotRepository>();
        services.AddScoped<ICaregiverDailyScheduleService, CaregiverDailyScheduleService>();
        services.AddScoped<ICaregiverDailyScheduleRepository, CaregiverDailyScheduleRepository>();
        services.AddScoped<ICaregiverStatusService, CaregiverStatusService>();
        services.AddScoped<ICaregiverStatusRepository, CaregiverStatusRepository>();
        return services;
    }

    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var dbSettings =
            configuration.GetSection(DbSettings.SectionName).Get<DbSettings>()
            ?? throw new InvalidOperationException(
                "DbConnectionStrings configuration section is missing"
            );

        services.Configure<DbSettings>(configuration.GetSection(DbSettings.SectionName));

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(dbSettings.ConnectionString)
        );

        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        return services;
    }

    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // Bind and validate JWT settings
        var jwtSettings =
            configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
            ?? throw new InvalidOperationException("JwtSettings configuration section is missing");

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Secret)
                    ),
                };

                // Read JWT from HttpOnly cookie
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies[CookieNames.Jwt];
                        return Task.CompletedTask;
                    },
                };
            });

        return services;
    }
}
