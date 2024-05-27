using KrosOrg.Hierarchia;
using Microsoft.EntityFrameworkCore;
namespace KrosOrg.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Division> Divisions { get; private set; }
        public DbSet<Company> Companies { get; private set; }
        public DbSet<Employee> Employees { get; private set; }
        public DbSet<Department> Departments { get; private set; }
        public DbSet<Project> Projects { get; private set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>()
                .HasOne(c => c.CEO)
                .WithMany()
                .HasForeignKey(c => c.CEOID)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }

    }
}
