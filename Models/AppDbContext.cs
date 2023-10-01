using Microsoft.EntityFrameworkCore;

namespace PostService.API.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    public DbSet<Post> Posts { get; set; }
}