﻿
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SLBS.Membership.Domain
{
    public class Mother
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }
        public int MemberId { get; set; }
        [DisplayName("Mothers Name")]
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        [Required]
        public virtual Member Member { get; set; }
    }
}