using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SLBS.Membership.Web.Models
{
    public class NoticeViewModel
    {
        public NoticeViewModel()
        {
            NoticeType = EnumNoticeTypes.PaymentStatusDhammaSchool;
        }

        public IEnumerable<Domain.Membership> Receipients { get; set; }
        public EnumNoticeTypes NoticeType { get; set; }
    }
}