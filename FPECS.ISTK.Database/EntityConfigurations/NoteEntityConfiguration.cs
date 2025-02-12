using FPECS.ISTK.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FPECS.ISTK.Database.EntityConfigurations;

public class NoteEntityConfiguration : IEntityTypeConfiguration<NoteEntity>
{
    public void Configure(EntityTypeBuilder<NoteEntity> builder)
    {
        builder
            .HasOne(x => x.User)
            .WithMany(x => x.Notes)
            .HasForeignKey(x => x.UserId);
    }
}
