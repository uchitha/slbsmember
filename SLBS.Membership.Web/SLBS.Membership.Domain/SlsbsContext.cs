using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLBS.Membership.Domain
{
    public class SlsbsContext : DbContext
    {
        public SlsbsContext() : base("Slsbs")
        {
            
        }

        public DbSet<Member> Members { get; set; }
        public DbSet<Mother> Mothers { get; set; }
        public DbSet<Father> Fathers { get; set; }
        public DbSet<Child> Children { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
