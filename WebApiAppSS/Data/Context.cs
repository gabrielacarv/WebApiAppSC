using Microsoft.EntityFrameworkCore;
using WebApiAppSS.Models;

namespace WebApiAppSS.Data
{
    public class Context : DbContext
    {
        public DbSet<User> User { get; set; }
        public DbSet<Group> Group { get; set; }
        public DbSet<Invitation> Invitation { get; set; }

        public Context(DbContextOptions<Context> options) : base(options)
        {
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(@"Data source = 10.107.176.41, 1434;
        //                            Database = BD044323; 
        //                            User ID = RA044323; 
        //                            Password = 044323;
        //                            TrustServerCertificate=true"
        //    );
        //}


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data source = 201.62.57.93, 1434;
                                    Database = BD044323; 
                                    User ID = RA044323; 
                                    Password = 044323;
                                    TrustServerCertificate=true"
            );
        }
        //}10.107.176.41,1434 201.62.57.93, 1434

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new GroupConfiguration());
            modelBuilder.ApplyConfiguration(new InvitationConfiguration());
        }
    }
}
