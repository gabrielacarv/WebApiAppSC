using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApiAppSS.Models;

namespace WebApiAppSS.Data
{
    public class DrawConfiguration : IEntityTypeConfiguration<Draw>
    {
        public void Configure(EntityTypeBuilder<Draw> builder)
        {
            builder.ToTable("Sorteio", "SchApp");
            builder.HasKey(d => d.DrawId);

            builder
                .Property(d => d.DrawId)
                .HasColumnName("SorteioID")
                .UseIdentityColumn()
                .HasColumnType("int");

            builder
                .Property(d => d.GroupId)
                .HasColumnName("GrupoID")
                .HasColumnType("int");

            builder
                .Property(d => d.ParticipantId)
                .HasColumnName("ParticipanteID")
                .HasColumnType("int");

            builder
                .Property(d => d.SelectedId)
                .HasColumnName("SorteadoID")
                .HasColumnType("int");

            builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(u => u.SelectedId);

            builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(u => u.ParticipantId);

            builder.HasOne<Group>()
               .WithMany()
               .HasForeignKey(g => g.GroupId);
        }
    }
}
