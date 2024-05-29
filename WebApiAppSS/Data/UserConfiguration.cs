using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApiAppSS.Models;

namespace WebApiAppSS.Data
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Usuarios", "SchApp");
            builder.HasKey(u => u.Id);

            builder
                .Property(u => u.Id)
                .HasColumnName("IdUsuario")
                .UseIdentityColumn()
                .HasColumnType("int");

            builder
                .Property(u => u.Name)
                .HasColumnName("Nome")
                .HasColumnType("varchar(50)");

            builder
                .Property(u => u.Email)
                .HasColumnName("Email")
                .HasColumnType("varchar(320)");

            builder
                .Property(u => u.Password)
                .HasColumnName("Senha")
                .HasColumnType("varchar(60)");

            builder
                .Property(u => u.Photo)
                .HasColumnName("Foto")
                .HasColumnType("varbinary(max)");
        }
    }
}
