using AnthillTest.UserService.Entities;
using Microsoft.EntityFrameworkCore;

namespace AnthillTest.UserService.Infrastructure.Persistence;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        
    }

    public DbSet<User> Users => Set<User>();
}
