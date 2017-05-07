using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace SLBS.Membership.Domain
{
    public class Membership
    {
        public Membership()
        {
            Children = new List<Child>();
            Adults = new List<Adult>();
            MembershipComments = new List<MembershipComment>();
            IsActive = true;
            BlockEmails = false;
        }

        [HiddenInput(DisplayValue = false)]
        public int MembershipId { get; set; }

        [DisplayName("Membership Number")]
        public string MembershipNumber { get; set; }


        [DisplayName("Contact Name")]
        public string ContactName { get; set; }

        [DisplayName("Payment Status")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? PaidUpTo { get; set; }

        [DisplayName("Application Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ApplicationDate { get; set; }

        [DisplayName("Emails Blocked")]
        public bool? BlockEmails { get; set; }

        //public string CurrentComment
        //{
        //    get { return MembershipComments.Any() ? MembershipComments.Last().Comment : string.Empty; }
        //    set 
        //    {

        //    }
        //}

        public bool IsActive { get; set; }
       
        public virtual ICollection<Child> Children { get; set; }
        public virtual ICollection<Adult> Adults { get; set; }
        public virtual ICollection<MembershipComment> MembershipComments { get; set; } 

    }
}
