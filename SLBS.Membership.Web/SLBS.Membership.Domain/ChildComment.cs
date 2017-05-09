using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SLBS.Membership.Domain
{
    public class ChildComment : IAuditable
    {
        [HiddenInput(DisplayValue = false)]
        public int ChildCommentId { get; set; }

        [ForeignKey("Child")]
        [Required]
        public int ChildId { get; set; }

        public string Comment { get; set; }

        public virtual Membership Membership { get; set; }
        [DisplayName("Created On")]
        public DateTime CreatedOn { get; set; }
        [DisplayName("Created By")]
        public string CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }

        public Child Child { get; set; }
    }
}
