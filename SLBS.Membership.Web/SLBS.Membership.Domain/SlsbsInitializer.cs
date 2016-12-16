using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLBS.Membership.Domain
{
    public class SlsbsInitializer : DropCreateDatabaseIfModelChanges<SlsbsContext>
    {
        protected override void Seed(SlsbsContext context)
        {
            var members = new List<Membership>()
            {
                new Membership {MembershipId = 1, MembershipNumber = "M001", ContactName = "Mr. & Mrs. M01"},
                new Membership {MembershipId = 2, MembershipNumber = "M002", ContactName = "Mr. & Mrs. M02"},
                new Membership {MembershipId = 3, MembershipNumber = "M003", ContactName = "Mr. & Mrs. M03"},
            };

            members.ForEach(m => context.Memberships.Add(m));
            context.SaveChanges();

            var mothers = new List<Adult>()
            {
                new Adult() {AdultId = 1, MembershipId = 1, FullName = "Mom 1", Role = MembershipRole.Mother},
                new Adult() {AdultId = 2, MembershipId = 2, FullName = "Mom 2", Role = MembershipRole.Mother},
                new Adult() {AdultId = 3, MembershipId = 3, FullName = "Mom 3", Role = MembershipRole.Mother},
            };

            mothers.ForEach(m => context.Adults.Add(m));
            context.SaveChanges();

            var fathers = new List<Adult>()
            {
                new Adult() {AdultId = 4, MembershipId = 1, FullName = "Dad 1", Role = MembershipRole.Father},
                new Adult() {AdultId = 5, MembershipId = 2, FullName = "Dad 2", Role = MembershipRole.Father},
                new Adult() {AdultId = 6, MembershipId = 3, FullName = "Dad 3", Role = MembershipRole.Father},
            };

            fathers.ForEach(m => context.Adults.Add(m));
            context.SaveChanges();

        }
    }
}
