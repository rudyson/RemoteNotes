using FPECS.ISTK.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace FPECS.ISTK.Database;
public class ApplicationDbContext : DbContext
{
    public virtual required DbSet<UserEntity> Users { get; set; }
    public virtual required DbSet<UserRoleEntity> Roles { get; set; }
    public virtual required DbSet<NoteEntity> Notes { get; set; }

    public ApplicationDbContext()
    {

    }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return base.SaveChangesAsync(cancellationToken);
    }
}
