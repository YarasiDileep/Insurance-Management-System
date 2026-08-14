using Microsoft.EntityFrameworkCore;
using Insurance.Core.Entities;

namespace Insurance.Infrastructure;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<Policy> Policies { get; set; } = null!;
    public DbSet<Claim> Claims { get; set; } = null!;

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
        });

        modelBuilder.Entity<Claim>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.ClaimNumber).IsRequired();
        });
    }
}
