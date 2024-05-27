using KrosOrg.Hierarchia;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace KrosOrg.Data
{
    public class DivisionConfiguration : IEntityTypeConfiguration<Division>
    {
        public void Configure(EntityTypeBuilder<Division> builder)
        {
            builder
                .HasOne<Employee>(x => x.DivisionLeader)
                .WithMany()
                .HasForeignKey(x => x.DivisionLeaderId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }

}
