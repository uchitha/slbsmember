using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using SLBS.Membership.Domain;
using SLBS.Membership.Web.Models;

namespace SLBS.Membership.Web.Data
{
    public static class ReportRepository
    {
        public static async Task<List<PayStatusViewModel>> GetPayStatusReport()
        {
            using (var context = new SlsbsContext())
            {
                var members = await context.Memberships.Where(m => !m.MembershipNumber.StartsWith("X")).OrderBy(m => m.MembershipNumber).ToListAsync();

                return
                    members.Select(
                        m =>
                            new PayStatusViewModel(m.MembershipNumber, m.ContactName,m.PaidUpTo.HasValue ? m.PaidUpTo.Value.ToString("yyyy-MM") : string.Empty)).ToList();

            }
        }

        public static async Task<List<MembershipDetailsViewModel>> GetMembershipDatails()
        {
            using (var context = new SlsbsContext())
            {
                var sqlString = GetMembershipDetailsSql();
                var mDetails = await context.Database.SqlQuery<MembershipDetailsViewModel>(sqlString).ToListAsync();

                return mDetails;

            }
        }

        public static async Task<List<DsChildDetailsViewModel>> GetChildrenDetails()
        {
            using (var context = new SlsbsContext())
            {
                var sqlString = GetDsChildDetailsSql();
                var childDetails = await context.Database.SqlQuery<DsChildDetailsViewModel>(sqlString).ToListAsync();

                return childDetails;

            }
        }

        private static string GetDsChildDetailsSql()
        {
            return
                @"select CASE WHEN c.ClassLevel = 10 THEN 'Senior' ELSE 'Level ' + Convert (varchar(10),c.ClassLevel) End ClassName, 
                MembershipNumber, c.FullName as ChildName, 
                CASE WHEN c.MediaConsent = 1 THEN 'YES' ELSE 'NO' END MediaConsent,
                CASE WHEN c.AmbulanceCover = 1 THEN 'YES' ELSE 'NO' END AmbulanceCover,
                LEFT(CONVERT(VARCHAR, PaidUpTo, 120), 10) PaymentStatus,
                FatherName, MotherName, FatherEmail, FatherMobile, FatherLandphone,MotherEmail,MotherMobile,MotherLandphone

                from Child c
                inner join 
                (select distinct Membership.MembershipId, Membership.MembershipNumber, Membership.PaidUpto, Father.FullName as FatherName, Mother.FullName as MotherName, 
                Father.Email as FatherEmail, Father.MobilePhone as FatherMobile, Father.LandPhone as FatherLandphone, 
                Mother.Email as MotherEmail, Mother.MobilePhone as MotherMobile, Mother.LandPhone as MotherLandphone
                from Membership
                left outer join
                (select m.MembershipId, m.MembershipNumber, a.FullName, a.MobilePhone, a.Email, a.LandPhone
                from Membership m left outer join Adult a on a.MembershipId = m.MembershipId
                where a.[Role] = 1) as Father
                on Father.MembershipId = Membership.MembershipId
                left outer join 
                (select m.MembershipId, m.MembershipNumber, a.FullName,a.MobilePhone, a.Email,a.LandPhone
                from Membership m left outer join Adult a on a.MembershipId = m.MembershipId
                where a.[Role] = 2) as Mother
                on Mother.MembershipId = Membership.MembershipId) as MemberDetails
                on MemberDetails.MembershipId = c.MembershipId 
                order by ClassLevel, MembershipNumber";
        }

        private static string GetMembershipDetailsSql()
        {
            return @"select m.MembershipNumber, m.ContactName,LEFT(CONVERT(VARCHAR, m.PaidUpTo, 120), 10) PaidUpTo,
                            Father.FullName as FathersName, Father.MobilePhone as FathersMobile, Father.Landphone as FathersLandphone, Father.Email as FathersEmail,
                            Mother.FullName as MothersName, Mother.MobilePhone as MothersMobile, Mother.Landphone as MothersLandphone, Mother.Email as MothersEmail
                            from Membership m
                            left outer join 
                            (select m.MembershipId, m.MembershipNumber, a.FullName, a.MobilePhone, a.Email, a.LandPhone
                            from Membership m left outer join Adult a on a.MembershipId = m.MembershipId
                            where a.[Role] = 1) as Father
                            on Father.MembershipId = m.MembershipId
                            left outer join 
                            (select m.MembershipId, m.MembershipNumber, a.FullName,a.MobilePhone, a.Email,a.LandPhone
                            from Membership m left outer join Adult a on a.MembershipId = m.MembershipId
                            where a.[Role] = 2) as Mother
                            on Mother.MembershipId = m.MembershipId

                            where m.MembershipNumber not like 'X%'
                            order by m.MembershipNumber
                            ";
        }
    }
}