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
            var members = new List<Member>()
            {
                new Member {Id = 1, MemberNo = "M001", FamilyName = "Mr. & Mrs. M01"},
                new Member {Id = 2, MemberNo = "M002", FamilyName = "Mr. & Mrs. M02"},
                new Member {Id = 3, MemberNo = "M003", FamilyName = "Mr. & Mrs. M03"},
            };

            members.ForEach(m => context.Members.Add(m));
            context.SaveChanges();

            var mothers = new List<Mother>()
            {
                new Mother() {Id = 1, MemberId = 1, Name = "Mom 1"},
                new Mother() {Id = 2, MemberId = 2, Name = "Mom 2"},
                new Mother() {Id = 3, MemberId = 3, Name = "Mom 3"},
            };

            mothers.ForEach(m => context.Mothers.Add(m));
            context.SaveChanges();

            var fathers = new List<Father>()
            {
                new Father() {Id = 1, MemberId = 1, Name = "Dad 1"},
                new Father() {Id = 2, MemberId = 2, Name = "Dad 2"},
                new Father() {Id = 3, MemberId = 3, Name = "Dad 3"},
            };

            fathers.ForEach(m => context.Fathers.Add(m));
            context.SaveChanges();


        }
    }
}
