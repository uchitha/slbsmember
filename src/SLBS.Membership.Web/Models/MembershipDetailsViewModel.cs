namespace SLBS.Membership.Web.Models
{
    public class MembershipDetailsViewModel
    {
        public string MembershipNumber { get; set; }
        public string ContactName { get; set; }
        public string PaidUpto { get; set; }
        public System.DateTime? LastNotificationDate { get; set; }

        public string FathersName { get; set; }
        public string FathersMobile { get; set; }
        public string FathersLandphone { get; set; }
        public string FathersEmail { get; set; }

        public string MothersName { get; set; }
        public string MothersMobile { get; set; }
        public string MothersLandphone { get; set; }
        public string MothersEmail { get; set; }

        public bool HasDsKids { get; set; }
    }
}