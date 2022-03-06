using Microsoft.EntityFrameworkCore;

namespace PortfolioAPI.Models;

public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
    }

    public DbSet<TodoItem> TodoItems { get; set; } = null!;
    public DbSet<Request> Requests { get; set; } = null!;
}