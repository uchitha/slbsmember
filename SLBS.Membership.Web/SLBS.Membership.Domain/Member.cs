using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SLBS.Membership.Domain
{
    public class Member
    {
        public Member()
        {
            Children = new List<Child>();
        }

        [ScaffoldColumn(false)]
        public int Id { get; set; }
        public int? MotherId { get; set; }  
        public int? FatherId { get; set; }
        [DisplayName("Member Number")]
        public string MemberNo { get; set; }

        [DisplayName("Family Name")]
        public string FamilyName { get; set; }

        public virtual Mother Mother { get; set; }
        public virtual Father Father { get; set; }
        public virtual ICollection<Child> Children { get; set; } 
    }
}
