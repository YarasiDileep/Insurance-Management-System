using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Insurance.Core.Entities;

namespace Insurance.Infrastructure;

// Extend IdentityDbContext so we can persist users and roles alongside domain entities
public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<Policy> Policies { get; set; } = null!;
    public DbSet<Claim> Claims { get; set; } = null!;
    public DbSet<Payment> Payments { get; set; } = null!;
    public DbSet<Document> Documents { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Customer>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Email).IsRequired();
            b.Property(x => x.FirstName).IsRequired();
            b.Property(x => x.LastName).IsRequired();
        });

        modelBuilder.Entity<Policy>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.PolicyNumber).IsRequired();
            // Ensure decimal precision for money fields
            b.Property(x => x.Premium).HasPrecision(18,2);
        });

        modelBuilder.Entity<Claim>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.ClaimNumber).IsRequired();
            // Ensure decimal precision for money fields
            b.Property(x => x.Amount).HasPrecision(18,2);
        });

        modelBuilder.Entity<Payment>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Amount).HasPrecision(18,2);
            b.Property(x => x.Method).IsRequired();
            b.Property(x => x.Reference).IsRequired(false);
        });

        modelBuilder.Entity<Document>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.FileName).IsRequired();
            b.Property(x => x.StoragePath).IsRequired();
        });
    }
}
