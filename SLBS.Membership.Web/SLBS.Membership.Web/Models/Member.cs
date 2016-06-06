using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SLBS.Membership.Web.Models
{
    public class Member
    {
        public string MemberNo { get; set; }
        public string MemberName { get; set; }
        public string Email { get; set; }
        public string PaymentStatus { get; set; }
        public decimal Payment { get; set; }
    }
}