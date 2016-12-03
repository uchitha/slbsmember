using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SLBS.Membership.Domain
{
    public class Child
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }
        public int MemberId { get; set; }
        [DisplayName("Child Name")]
        public string Name { get; set; }
        [DisplayName("Class Level")]
        public ClassLevelEnum ClassLevel { get; set; }

        [DisplayName("Media Consent")]
        public bool? MediaConsent { get; set; }
        [DisplayName("Ambulance Cover")]
        public bool? AmbulanceCover { get; set; }

        [DisplayName("Payment Status")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? PaidUpTo { get; set; }
        public virtual Member Member { get; set; }
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
        Level6
    }
}
