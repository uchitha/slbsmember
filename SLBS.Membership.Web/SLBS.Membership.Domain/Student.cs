namespace SLBS.Membership.Domain
{
    public class Student
    {
        public int StudentId { get; set; }
        public string Name { get; set; }

        public int? StudentAddressId { get; set; }
        public virtual StudentAddress   StudentAddress { get; set; }

    }
}
