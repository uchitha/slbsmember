using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace SLBS.Membership.Domain
{
    public class SlsbsInitializer : DropCreateDatabaseAlways<SlsbsContext>
    {
        protected override void Seed(SlsbsContext context)
        {
            var members = new List<Membership>()
            {
                new Membership {MembershipId = 1, MembershipNumber = "W041", ContactName = "Chandana Weerasekera Family", PaidUpTo = new DateTime(2016,1,1)},
                new Membership {MembershipId = 2, MembershipNumber = "G022", ContactName = "Pramodh Gunawardena Family", PaidUpTo = new DateTime(2016,4,1)},
                new Membership {MembershipId = 3, MembershipNumber = "R027", ContactName = "Nilakshi & Uchitha", PaidUpTo = new DateTime(2015,12,1)},
            };

            members.ForEach(m => context.Memberships.Add(m));
            context.SaveChanges();

            var mothers = new List<Adult>()
            {
                new Adult() {AdultId = 1, MembershipId = 1, FullName = "Varuni Weerasekara", Role = MembershipRole.Mother, Email = "cargillsvaruni@yahoo.co.uk"},
                new Adult() {AdultId = 2, MembershipId = 2, FullName = "Wasantha Gunawardhana", Role = MembershipRole.Mother, Email = "w.gunawardhana@gmail.com"},
                new Adult() {AdultId = 3, MembershipId = 3, FullName = "Nilakshi Abeysinghe", Role = MembershipRole.Mother, Email = "nilakshia@live.com"},
            };

            mothers.ForEach(m => context.Adults.Add(m));
            context.SaveChanges();

            var fathers = new List<Adult>()
            {
                new Adult() {AdultId = 4, MembershipId = 1, FullName = "Chandana Weerasekera", Role = MembershipRole.Father, Email = "Chaweea8@yahoo.co.uk"},
                new Adult() {AdultId = 5, MembershipId = 2, FullName = "Pramodh Gunwardhana", Role = MembershipRole.Father, Email = "p.gunawardhana@curtin.com.au"},
                new Adult() {AdultId = 6, MembershipId = 3, FullName = "Uchitha Ranasinghe", Role = MembershipRole.Father, Email = "uchitha.r@gmail.com"},
            };

            fathers.ForEach(m => context.Adults.Add(m));
            context.SaveChanges();

            var kids = new List<Child>()
            {
                new Child(){ChildId = 1, MembershipId = 1, FullName = "Rashini Weerasekara",AmbulanceCover = true,MediaConsent = false,ClassLevel = ClassLevelEnum.Level6},
                new Child(){ChildId = 2, MembershipId = 2, FullName = "Nisani Gunawardhana",AmbulanceCover = false,MediaConsent = true,ClassLevel = ClassLevelEnum.Level2},
                 new Child(){ChildId = 3, MembershipId = 2, FullName = "Binithi Gunawardhana",AmbulanceCover = false,MediaConsent = true,ClassLevel = ClassLevelEnum.Level3},
                new Child(){ChildId = 4, MembershipId = 3, FullName = "Thumindu Ranasinghe",AmbulanceCover = true,MediaConsent = true,ClassLevel = ClassLevelEnum.Level1}
            };

            kids.ForEach(k => context.Children.Add(k));
            context.SaveChanges();

        }
    }
}
