using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApiAppSS.Models;

namespace WebApiAppSS.Data
{
    public class InvitationConfiguration : IEntityTypeConfiguration<Invitation>
    {
        public void Configure(EntityTypeBuilder<Invitation> builder)
        {
            builder.ToTable("Convite", "SchApp");
            builder.HasKey(c => c.IdInvitation);

            builder
                .Property(c => c.IdInvitation)
                .HasColumnName("IdConvite")
                .UseIdentityColumn()
                .HasColumnType("int");

            builder
                .Property(c => c.GroupId)
                .HasColumnName("GrupoId")
                .HasColumnType("int");

            builder
                .Property(c => c.RecipientId)
                .HasColumnName("DestinatarioId")
                .HasColumnType("int");

            builder
                .Property(c => c.SenderId)
                .HasColumnName("RemetenteId")
                .HasColumnType("int");

            builder
                .Property(c => c.Status)
                .HasColumnName("Status")
                .HasColumnType("varchar(50)");

            builder.HasOne<Group>()
               .WithMany()
               .HasForeignKey(c => c.GroupId);

            builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(c => c.RecipientId);

            builder.HasOne<User>()
              .WithMany()
              .HasForeignKey(c => c.SenderId);
        }
    }
}
