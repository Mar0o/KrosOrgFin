using System.ComponentModel.DataAnnotations.Schema;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace KrosOrg.Hierarchia
{
    public class Division
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int CompanyId { get; set; }

        public Company Company { get; set; }

        public string Name { get; set; }

        public int? DivisionLeaderId { get; set; }

        public Employee? DivisionLeader { get; set; }
        public ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}
