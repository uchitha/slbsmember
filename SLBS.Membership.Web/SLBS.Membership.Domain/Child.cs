using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace SLBS.Membership.Domain
{
    public class Child
    {
        [HiddenInput(DisplayValue = false)]
        public int ChildId { get; set; }

        [HiddenInput(DisplayValue = false)]
        [ForeignKey("Membership")]
        [Required]
        public int MembershipId { get; set; }
        [DisplayName("Child Name")]
        public string FullName { get; set; }
        [DisplayName("Class Level")]
        public ClassLevelEnum ClassLevel { get; set; }

        [DisplayName("Media Consent")]
        public bool? MediaConsent { get; set; }
        [DisplayName("Ambulance Cover")]
        public bool? AmbulanceCover { get; set; }

       
        public virtual Membership Membership { get; set; }
    }

    public enum ClassLevelEnum
    {
        [Description("DS Level 1")]
        Level1,
        [Description("DS Level 2")]
        Level2,
        [Description("DS Level 3")]
        Level3,
        [Description("DS Level 4")]
        Level4,
        [Description("DS Level 5")]
        Level5,
        [Description("DS Level 6")]
        Level6,
        [Description("DS Level 7")]
        Level7
    }
}
