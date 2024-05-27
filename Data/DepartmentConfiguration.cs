using KrosOrg.Hierarchia;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace KrosOrg.Data
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder
                .HasOne<Employee>(x => x.DepartmentLeader)
                .WithMany()
                .HasForeignKey(x => x.DepartmentLeaderId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
