using auth_abac.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace auth_abac.Databases
{
    public class DataContext : DbContext
    {
        public DbSet<Department> Departments { get; set; } = null!;
        public DbSet<Position> Positions { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;

        public DataContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Position>().HasData(
                new Position { Id = 1, Name = "President" },
                new Position { Id = 2, Name = "Sales Manager" },
                new Position { Id = 3, Name = "Customer Support" }
            );

            modelBuilder.Entity<Department>().HasData(
                new Department { Id = 1, Name = "IT" },
                new Department { Id = 2, Name = "R&D" }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Timestamp = new DateOnly(2012, 06, 04), DepartmentId = 1, Name = "Cloud services and solutions" },
                new Product { Id = 2, Timestamp = new DateOnly(2016, 04, 13), DepartmentId = 1, Name = "Big Data Analysis Tools" },
                new Product { Id = 3, Timestamp = new DateOnly(2017, 09, 27), DepartmentId = 1, Name = "Test automation systems" },
                new Product { Id = 4, Timestamp = new DateOnly(2019, 12, 01), DepartmentId = 1, Name = "Internal libraries and frameworks" },
                
                new Product { Id = 5, Timestamp = new DateOnly(2015, 11, 02), DepartmentId = 2, Name = "Research in Quantum Computing" },
                new Product { Id = 6, Timestamp = new DateOnly(2018, 08, 15), DepartmentId = 2, Name = "Development of new methods of encryption" },
                new Product { Id = 7, Timestamp = new DateOnly(2022, 05, 03), DepartmentId = 2, Name = "Creation of automation systems for production" },
                new Product { Id = 8, Timestamp = new DateOnly(2024, 03, 10), DepartmentId = 2, Name = "Research in genetics and bioinformatics" }
            );

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, EnrollmentDate = new DateOnly(2010, 10, 02), PositionId = 1,                   Name = "Tom" },
                
                new User { Id = 2, EnrollmentDate = new DateOnly(2012, 05, 08), PositionId = 2, DepartmentId = 1, Name = "Bob" },
                new User { Id = 3, EnrollmentDate = new DateOnly(2017, 02, 20), PositionId = 3, DepartmentId = 1, Name = "Alice" },
                new User { Id = 4, EnrollmentDate = new DateOnly(2022, 12, 20), PositionId = 3, DepartmentId = 1, Name = "Sam" },

                new User { Id = 5, EnrollmentDate = new DateOnly(2013, 01, 07), PositionId = 2, DepartmentId = 2, Name = "Jack" },
                new User { Id = 6, EnrollmentDate = new DateOnly(2017, 07, 12), PositionId = 3, DepartmentId = 2, Name = "Fred" },
                new User { Id = 7, EnrollmentDate = new DateOnly(2024, 01, 23), PositionId = 3, DepartmentId = 2, Name = "John" }
            );
        }
    }
}
