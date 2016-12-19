using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace SLBS.Membership.Domain
{
    public class SlsbsInitializer : DropCreateDatabaseIfModelChanges<SlsbsContext>
    {
        protected override void Seed(SlsbsContext context)
        {
            var members = new List<Membership>()
            {
                new Membership {MembershipId = 1, MembershipNumber = "M001", ContactName = "Mr. & Mrs. M01", PaidUpTo = new DateTime(2016,1,1)},
                new Membership {MembershipId = 2, MembershipNumber = "M002", ContactName = "Mr. & Mrs. M02", PaidUpTo = new DateTime(2016,4,1)},
                new Membership {MembershipId = 3, MembershipNumber = "M003", ContactName = "Mr. & Mrs. M03", PaidUpTo = new DateTime(2016,12,1)},
            };

            members.ForEach(m => context.Memberships.Add(m));
            context.SaveChanges();

            var mothers = new List<Adult>()
            {
                new Adult() {AdultId = 1, MembershipId = 1, FullName = "Mom 1", Role = MembershipRole.Mother, Email = "mom1@gmail.com"},
                new Adult() {AdultId = 2, MembershipId = 2, FullName = "Mom 2", Role = MembershipRole.Mother, Email = "mom2@gmail.com"},
                new Adult() {AdultId = 3, MembershipId = 3, FullName = "Mom 3", Role = MembershipRole.Mother, Email = "mom3@gmail.com"},
            };

            mothers.ForEach(m => context.Adults.Add(m));
            context.SaveChanges();

            var fathers = new List<Adult>()
            {
                new Adult() {AdultId = 4, MembershipId = 1, FullName = "Dad 1", Role = MembershipRole.Father, Email = "dad1@gmail.com"},
                new Adult() {AdultId = 5, MembershipId = 2, FullName = "Dad 2", Role = MembershipRole.Father, Email = "dad2@gmail.com"},
                new Adult() {AdultId = 6, MembershipId = 3, FullName = "Dad 3", Role = MembershipRole.Father, Email = "dad3@gmail.com"},
            };

            fathers.ForEach(m => context.Adults.Add(m));
            context.SaveChanges();

            var kids = new List<Child>()
            {
                new Child(){ChildId = 1, MembershipId = 1, FullName = "Child1 M001",AmbulanceCover = true,MediaConsent = false,ClassLevel = ClassLevelEnum.Level1},
                new Child(){ChildId = 2, MembershipId = 1, FullName = "Child2 M001",AmbulanceCover = true,MediaConsent = false,ClassLevel = ClassLevelEnum.Level2},
                new Child(){ChildId = 3, MembershipId = 2, FullName = "Child1 M002",AmbulanceCover = false,MediaConsent = true,ClassLevel = ClassLevelEnum.Level2},
                new Child(){ChildId = 4, MembershipId = 2, FullName = "Child2 M002",AmbulanceCover = false,MediaConsent = true,ClassLevel = ClassLevelEnum.Level3},
                new Child(){ChildId = 5, MembershipId = 2, FullName = "Child3 M002",AmbulanceCover = false,MediaConsent = true,ClassLevel = ClassLevelEnum.Level5},
                new Child(){ChildId = 6, MembershipId = 3, FullName = "Child1 M003",AmbulanceCover = true,MediaConsent = true,ClassLevel = ClassLevelEnum.Level1}
            };

            kids.ForEach(k => context.Children.Add(k));
            context.SaveChanges();

        }
    }
}
