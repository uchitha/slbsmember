
namespace SLBS.Membership.Domain
{
    public class StudentAddress
    {
        public int StudentAddressId { get; set; }
        public string Address { get; set; }

        public int StudentId { get; set; }

        public virtual Student Student { get; set; }
    }
}
