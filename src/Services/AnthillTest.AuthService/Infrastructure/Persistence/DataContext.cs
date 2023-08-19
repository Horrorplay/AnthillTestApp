using AnthillTest.AuthService.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AnthillTest.AuthService.Infrastructure.Persistence;

public class DataContext : IdentityDbContext<AppUser, IdentityRole<long>, long>
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        
    }

    public DbSet<Application> Applications { get; set; } = null!;
}
