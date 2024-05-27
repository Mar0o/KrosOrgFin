using KrosOrg.Hierarchia;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace KrosOrg.Data
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder
                .HasOne<Employee>(x => x.ProjectLeader)
                .WithMany()
                .HasForeignKey(x => x.ProjectLeaderId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }

}
