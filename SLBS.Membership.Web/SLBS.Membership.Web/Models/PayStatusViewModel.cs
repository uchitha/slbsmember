namespace SLBS.Membership.Web.Models
{
    public class PayStatusViewModel
    {
        public PayStatusViewModel(string memberNumber, string contactDetails, string paidUpto)
        {
            MembershipNumber = memberNumber;
            MembershipDetails = contactDetails;
            PaidUpto = paidUpto;
        }
        public string MembershipNumber { get; set; }
        public string MembershipDetails { get; set; }
        public string PaidUpto { get; set; }
    }
}