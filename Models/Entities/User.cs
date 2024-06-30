namespace auth_abac.Models.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateOnly EnrollmentDate { get; set; }
        public int PositionId { get; set; }
        public int? DepartmentId { get; set; }

        public Position Position { get; set; }
        public Department Department { get; set; }
    }
}
