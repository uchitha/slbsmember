using System.Collections.Generic;
using System.Web.Http;
using Newtonsoft.Json;
using SLBS.Membership.Domain;

namespace SLBS.Membership.Web.Api
{
    public class MembershipSearchController : ApiController
    {
        public List<Member> Get(string name)
        {
            var result = new List<Member>
            {
                new Member
                {
                    FamilyName = "foo",
                    MemberNo = "M110"
                },
                new Member
                {
                    FamilyName = "foo bar",
                    MemberNo = "R110"
                }
            };

            return result;
        }
    }
}
