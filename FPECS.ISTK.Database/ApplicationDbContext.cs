using FPECS.ISTK.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace FPECS.ISTK.Database;
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public required DbSet<UserEntity> Users { get; set; }
    public required DbSet<UserRoleEntity> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return base.SaveChangesAsync(cancellationToken);
    }
}
