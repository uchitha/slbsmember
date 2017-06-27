using System.ComponentModel;

namespace SLBS.Membership.WebJobEmail
{
    public enum EnumNoticeTypes
    {
        [Description("Payment Status")]
        PaymentStatus = 1,
        [Description("New Member Welcome")]
        NewMember = 2,
      //  PaymentStatusSlsbsMembership = 2
    }
}