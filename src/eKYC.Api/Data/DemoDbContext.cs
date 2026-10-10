using eKYC.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace eKYC.Api.Data;

public sealed class DemoDbContext(DbContextOptions<DemoDbContext> options) : DbContext(options)
{
    public DbSet<ConsentRecord> Consents => Set<ConsentRecord>();
    public DbSet<KycRequest> KycRequests => Set<KycRequest>();
    public DbSet<RequestEvent> RequestEvents => Set<RequestEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ConsentRecord>().HasIndex(x => new { x.CustomerReference, x.Purpose }).IsUnique();
        modelBuilder.Entity<KycRequest>().HasIndex(x => x.Status);
        modelBuilder.Entity<RequestEvent>().HasIndex(x => x.KycRequestId);
    }
}
