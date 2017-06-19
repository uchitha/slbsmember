using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLBS.Membership.Domain
{
    public class SlsbsEmailObject
    {
        public SlsbsEmailObject(Membership member, string email)
        {
            Member = member;
            Email = email;
        }

        public Membership Member { get; set; }
        public string Email { get; set; }
    }
}
