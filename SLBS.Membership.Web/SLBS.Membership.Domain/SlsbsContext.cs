using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
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

            modelBuilder
                .Entity<Membership>()
                .Property(t => t.MembershipNumber)
                .IsRequired()
                .HasMaxLength(60)
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(
                        new IndexAttribute("IX_MembershipNumber", 1) { IsUnique = true }));


        }
    }
}
