using AnthillTest.UserService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AnthillTest.UserService.Extensions;

public static class DbContextExtension
{
    public static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefConnection");

        services.AddDbContextFactory<DataContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });
    }
}
