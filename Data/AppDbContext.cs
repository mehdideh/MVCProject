using Microsoft.EntityFrameworkCore;
using MVCProject.Models;

namespace MVCProject.Data
{

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Employee> Employees { get; set; }
    }
}

