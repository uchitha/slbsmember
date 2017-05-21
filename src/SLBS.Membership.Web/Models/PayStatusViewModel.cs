namespace SLBS.Membership.Web.Models
{
    public class PayStatusViewModel
    {
        public PayStatusViewModel()
        {
            
        }

        public PayStatusViewModel(string memberNumber, string contactDetails, string paidUpto, string lastNotificationDate)
        {
            MembershipNumber = memberNumber;
            ContactName = contactDetails;
            PaidUpto = paidUpto;
            LastNotificationDate = lastNotificationDate;

        }
        public string MembershipNumber { get; set; }
        public string ContactName { get; set; }
        public string PaidUpto { get; set; }
        public string LastNotificationDate { get; set; }
    }
}