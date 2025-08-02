using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MVCProject.Data;

namespace MVCProject.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer("Server=DESKTOP-M42QD3R\\SQLEXPRESS;Database=MvcDb;Trusted_Connection=True;TrustServerCertificate=True");
        return new AppDbContext(optionsBuilder.Options);
    }
}

