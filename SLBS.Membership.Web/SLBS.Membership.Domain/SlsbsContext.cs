using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration.Conventions;
using Microsoft.AspNet.Identity.EntityFramework;
using SLBS.Membership.Domain.Identity;

namespace SLBS.Membership.Domain
{
    public class SlsbsContext : IdentityDbContext<AppUser>
    {
        public SlsbsContext() : base("Slsbs")
        {
            Database.SetInitializer(new SlsbsInitializer());
        }

        public DbSet<Membership> Memberships { get; set; }
        public DbSet<Adult> Adults { get; set; }
        public DbSet<Child> Children { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
