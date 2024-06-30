namespace auth_abac.Models.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateOnly Timestamp { get; set; }
        public int DepartmentId { get; set; }

        public Department Department { get; set; }
    }
}
