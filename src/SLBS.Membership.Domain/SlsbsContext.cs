﻿using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration.Conventions;
using Microsoft.AspNet.Identity.EntityFramework;
using SLBS.Membership.Domain.Identity;

namespace SLBS.Membership.Domain
{
    public class SlsbsContext : IdentityDbContext<ApplicationUser>
    {
        public SlsbsContext() : base("Slsbs")
        {
            //Initializer is set in the App_Start
        }

        public static SlsbsContext Create()
        {
            return new SlsbsContext();
        }

        public DbSet<Membership> Memberships { get; set; }
        //public DbSet<Membership> MembershipComments { get; set; }
        public DbSet<Adult> Adults { get; set; }
        public DbSet<Child> Children { get; set; }
        public DbSet<ChildComment> ChlidComments { get; set; }

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
