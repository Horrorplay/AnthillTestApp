using AnthillTest.API_Gateway.Middlewares;

namespace AnthillTest.API_Gateway.Extensions;

public static class MiddlewareExtension
{
    public static void ConfigureCustomExceptionMiddleware(this WebApplication app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
    }
}
