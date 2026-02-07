using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Add DbSet<T> here later, e.g.
    public DbSet<Participant> Participants => Set<Participant>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Participant>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.FullName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(x => x.NdisNumber)
                .HasMaxLength(50);

            entity.Property(x => x.Email)
                .HasMaxLength(200);

            entity.Property(x => x.Phone)
                .HasMaxLength(50);

            entity.Property(x => x.Status)
                .IsRequired()
                .HasMaxLength(30);

            // 可选：常用索引
            entity.HasIndex(x => x.NdisNumber);
            entity.HasIndex(x => x.Email);
        });

        modelBuilder.Entity<Provider>(entity =>
{
    entity.HasKey(x => x.Id);

    entity.Property(x => x.Name)
        .IsRequired()
        .HasMaxLength(200);

    entity.Property(x => x.Abn)
        .HasMaxLength(50);

    entity.Property(x => x.ContactEmail)
        .HasMaxLength(200);

    entity.Property(x => x.ContactPhone)
        .HasMaxLength(50);

    entity.Property(x => x.Status)
        .IsRequired()
        .HasMaxLength(30);

    entity.HasIndex(x => x.Abn);
});
    }
}