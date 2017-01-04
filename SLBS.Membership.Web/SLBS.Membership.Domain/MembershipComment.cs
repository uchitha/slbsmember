using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace SLBS.Membership.Domain
{
    public class MembershipComment
    {
        [HiddenInput(DisplayValue = false)]
        public int MembershipCommentId { get; set; }

        [ForeignKey("Membership")]
        [Required]
        public int MembershipId { get; set; }

        public string Comment { get; set; }
        public DateTime CommentedOn { get; set; }
        public string CreatedBy { get; set; }

        public string CurrentStatus { get; set; }
        public DateTime StatusUpdatedOn { get; set; }
        public string StatusUpdatedBy { get; set; }

        public virtual Membership Membership { get; set; }


    }
}
