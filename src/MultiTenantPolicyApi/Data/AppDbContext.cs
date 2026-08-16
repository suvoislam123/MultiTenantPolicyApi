using Microsoft.EntityFrameworkCore;
using MultiTenantPolicyApi.Models;
using System.Reflection.Emit;

namespace MultiTenantPolicyApi.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Policy> Policies => Set<Policy>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.HasKey(t => t.Id);

            entity.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.HasIndex(t => t.Name)
                .IsUnique();
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(c => c.Id);

            entity.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.HasOne(c => c.Tenant)
                .WithMany(t => t.Customers)
                .HasForeignKey(c => c.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(c => new
            {
                c.TenantId,
                c.Id
            });
        });

        modelBuilder.Entity<Policy>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.Property(p => p.PolicyNumber)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(p => p.PremiumAmount)
                .HasPrecision(18, 2);

            entity.HasOne(p => p.Customer)
                .WithMany(c => c.Policies)
                .HasForeignKey(p => p.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(p => p.PolicyNumber)
                .IsUnique();

            entity.HasIndex(p => new
            {
                p.CustomerId,
                p.ExpirationDate
            });
        });
    }
}