namespace SLBS.Membership.Web.Models
{
    public class PayStatusViewModel
    {
        public PayStatusViewModel()
        {
            
        }

        public PayStatusViewModel(string memberNumber, string contactDetails, string paidUpto)
        {
            MembershipNumber = memberNumber;
            ContactName = contactDetails;
            PaidUpto = paidUpto;
        }
        public string MembershipNumber { get; set; }
        public string ContactName { get; set; }
        public string PaidUpto { get; set; }
    }
}