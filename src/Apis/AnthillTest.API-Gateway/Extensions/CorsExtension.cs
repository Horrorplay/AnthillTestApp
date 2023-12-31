﻿namespace AnthillTest.API_Gateway.Extensions;

public static class CorsExtension
{
    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder => builder.SetIsOriginAllowed((host) => true)
                .AllowCredentials()
                .AllowAnyHeader()
                .AllowAnyMethod());
        });
    }
}
