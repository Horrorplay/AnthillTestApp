using AnthillTest.AuthService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AnthillTest.AuthService.Extensions;

public static class DbContextExtension
{
    public static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DevConnection");

        services.AddDbContextFactory<DataContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });
    }
}
