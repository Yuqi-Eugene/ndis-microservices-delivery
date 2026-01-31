using Microsoft.EntityFrameworkCore;

namespace Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Add DbSet<T> here later, e.g.
    // public DbSet<Participant> Participants => Set<Participant>();
}