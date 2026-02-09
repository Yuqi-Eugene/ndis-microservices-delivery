using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Add DbSet<T> here later, e.g.
    public DbSet<Participant> Participants => Set<Participant>();
    public DbSet<Provider> Providers => Set<Provider>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<ServiceDelivery> ServiceDeliveries => Set<ServiceDelivery>();
    public DbSet<Claim> Claims => Set<Claim>();

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

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.ServiceType)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(x => x.Status)
                .IsRequired()
                .HasMaxLength(30);

            entity.Property(x => x.Notes)
                .HasMaxLength(2000);

            // Relationships
            entity.HasOne(x => x.Participant)
                .WithMany()
                .HasForeignKey(x => x.ParticipantId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Provider)
                .WithMany()
                .HasForeignKey(x => x.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Helpful indexes for filtering
            entity.HasIndex(x => x.ParticipantId);
            entity.HasIndex(x => x.ProviderId);
            entity.HasIndex(x => x.ScheduledStartUtc);
        });

        modelBuilder.Entity<ServiceDelivery>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Status)
                .IsRequired()
                .HasMaxLength(30);

            entity.Property(x => x.Notes)
                .HasMaxLength(2000);

            entity.HasOne(x => x.Booking)
                .WithMany() // 暂时不在 Booking 上加集合，保持简单
                .HasForeignKey(x => x.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => x.BookingId);
            entity.HasIndex(x => x.ActualStartUtc);
            entity.HasIndex(x => x.Status);
        });

        modelBuilder.Entity<Claim>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Amount)
                .HasColumnType("decimal(10,2)");

            entity.Property(x => x.Status)
                .IsRequired()
                .HasMaxLength(30);

            entity.HasOne(x => x.ServiceDelivery)
                .WithMany()
                .HasForeignKey(x => x.ServiceDeliveryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => x.ServiceDeliveryId)
                .IsUnique(); // 一个 delivery 只能生成一个 claim
        });
    }
}