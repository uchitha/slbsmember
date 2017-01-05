using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace SLBS.Membership.Domain
{
    public class Adult
    {
        public Adult()
        {
            IsActive = true;
        }

        [HiddenInput(DisplayValue = false)]
        public int AdultId { get; set; }

        [ForeignKey("Membership")]
        [Required]
        public int MembershipId { get; set; }

        [DisplayName("Fulll Name")]
        public string FullName { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }
        
        public string Email { get; set; }

        public bool IsActive { get; set; }

        public MembershipRole Role { get; set; }

        public virtual Membership Membership { get; set; }
    }

    public enum MembershipRole
    {
        General = 0,
        Father,
        Mother
    }
}
