using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace SLBS.Membership.Domain
{
    public class SlsbsContext : DbContext
    {
        public SlsbsContext() : base("Slsbs")
        {
            
        }

        public DbSet<Membership> Memberships { get; set; }
        public DbSet<Adult> Adults { get; set; }
        public DbSet<Child> Children { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
