using Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Api.Auth;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Api.Data;

// AppDbContext is the EF Core gateway to the database.
// It defines which entities are stored and how they map to tables, columns, keys, and indexes.
public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Each DbSet<T> represents a queryable table-like entry point.
    public DbSet<Participant> Participants => Set<Participant>();
    public DbSet<Provider> Providers => Set<Provider>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<ServiceDelivery> ServiceDeliveries => Set<ServiceDelivery>();
    public DbSet<Claim> Claims => Set<Claim>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Important: call the Identity base configuration first so ASP.NET Identity tables are mapped correctly.
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Participant>(entity =>
        {
            // Primary key.
            entity.HasKey(x => x.Id);

            // Required field + max length becomes both a business constraint and a database schema rule.
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

            // Indexes improve lookup speed for common filters/searches.
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

            // A booking must reference an existing participant.
            // Restrict delete avoids accidentally deleting a participant that is still used by a booking.
            entity.HasOne(x => x.Participant)
                .WithMany()
                .HasForeignKey(x => x.ParticipantId)
                .OnDelete(DeleteBehavior.Restrict);

            // A booking must also reference an existing provider.
            entity.HasOne(x => x.Provider)
                .WithMany()
                .HasForeignKey(x => x.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            // These indexes support common query patterns.
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

            // Every delivery belongs to one booking.
            entity.HasOne(x => x.Booking)
                .WithMany() // No collection on Booking yet; the model stays intentionally simple.
                .HasForeignKey(x => x.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => x.BookingId);
            entity.HasIndex(x => x.ActualStartUtc);
            entity.HasIndex(x => x.Status);
            // OwnerUserId links the delivery to the authenticated Identity user who created it.
            entity.Property(x => x.OwnerUserId)
                .IsRequired()
                .HasMaxLength(450);

            entity.HasIndex(x => x.OwnerUserId);
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
                .IsUnique(); // Enforces the business rule: one delivery can produce only one claim.
        });
    }
}
