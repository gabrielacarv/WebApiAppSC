using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApiAppSS.Models;

namespace WebApiAppSS.Data
{
    public class GroupConfiguration : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> builder)
        {
            builder.ToTable("Grupo", "SchApp");
            builder.HasKey(g => g.IdGroup);

            builder
                .Property(u => u.IdGroup)
                .HasColumnName("IdGrupo")
                .UseIdentityColumn()
                .HasColumnType("int");

            builder
                .Property(g => g.Name)
                .HasColumnName("Nome")
                .HasColumnType("varchar(50)");

            builder
                .Property(g => g.MaxPeople)
                .HasColumnName("MaxPessoas")
                .HasColumnType("int");

            builder
                .Property(g => g.DisclosureDate)
                .HasColumnName("DataRevelacao")
                .HasColumnType("date");

            builder
                .Property(g => g.Value)
                .HasColumnName("Valor")
                .HasColumnType("decimal");

            builder
                .Property(g => g.Description)
                .HasColumnName("Descricao")
                .HasColumnType("varchar(200)");

            builder
                .Property(g => g.Administrator)
                .HasColumnName("Administrador")
                .HasColumnType("int");

            builder
                .Property(g => g.Icon)
                .HasColumnName("Icone")
                .HasColumnType("varbinary(max)");

            builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(g => g.Administrator);
        }
    }
}
