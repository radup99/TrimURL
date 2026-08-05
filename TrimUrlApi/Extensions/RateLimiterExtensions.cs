using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace TrimUrlApi.Extensions;

public static class RateLimiterExtensions
{
    public static IServiceCollection AddRateLimitingPolicies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode =
                StatusCodes.Status429TooManyRequests;

            options.AddPolicy(RateLimitPolicies.Authentication, context =>
            {
                var ip = GetIpAddress(context);

                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: ip,
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = configuration.GetValue(
                            "RateLimiting:Authentication:PermitLimit",
                            5),
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    });
            });

            options.AddPolicy(RateLimitPolicies.UrlCreation, context =>
            {
                var ip = GetIpAddress(context);

                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: ip,
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = configuration.GetValue(
                            "RateLimiting:Authentication:UrlCreation",
                            20),
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    });
            });

            options.AddPolicy(RateLimitPolicies.GeneralApi, context =>
            {
                var ip = GetIpAddress(context);

                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: ip,
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = configuration.GetValue(
                            "RateLimiting:Authentication:GeneralApi",
                            100),
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    });
            });
        });

        return services;
    }

    private static string GetIpAddress(HttpContext context)
    {
        return context.Connection.RemoteIpAddress?.ToString()
            ?? "unknown";
    }
}