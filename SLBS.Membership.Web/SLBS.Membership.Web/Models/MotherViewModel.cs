using System.ComponentModel.DataAnnotations;
using SLBS.Membership.Domain;

namespace SLBS.Membership.Web.Models
{
    public class MotherViewModel
    {
        public int Id { get; set; }
        [Required]
        public int MemberId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public Mother ToDbMother()
        {
            return new Mother
            {
                Id = Id,
                MemberId = MemberId,
                Name = Name,
                Phone = Phone,
                Email = Email
            };
        }

        public MotherViewModel ToVmMother(Mother dbMother)
        {
            return new MotherViewModel
            {
                Id = Id,
                MemberId = MemberId,
                Name = Name,
                Phone = Phone,
                Email = Email
            };
        }
    }
}