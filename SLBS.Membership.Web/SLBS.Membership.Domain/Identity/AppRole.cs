using Microsoft.AspNet.Identity.EntityFramework;

namespace SLBS.Membership.Domain.Identity
{
    public class AppRole : IdentityRole
    {
        public AppRole() : base() { }
        public AppRole(string name) : base(name) { }
        // extra properties here 
    }
}
