using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SLBS.Membership.Domain
{
    public class Membership
    {
        public Membership()
        {
            Children = new List<Child>();
            Adults = new List<Adult>();
        }

        [HiddenInput(DisplayValue = false)]
        public int MembershipId { get; set; }

        [DisplayName("Membership Number")]
        public string MembershipNumber { get; set; }


        [DisplayName("Contact Name")]
        public string ContactName { get; set; }

        [DisplayName("Payment Status")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM}", ApplyFormatInEditMode = true)]
        public DateTime? PaidUpTo { get; set; }

       
        public virtual ICollection<Child> Children { get; set; }
        public virtual ICollection<Adult> Adults { get; set; } 

    }
}
