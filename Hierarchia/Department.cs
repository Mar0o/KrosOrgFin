using System.ComponentModel.DataAnnotations.Schema;

namespace KrosOrg.Hierarchia
{
    public class Department
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public Project Project { get; set; }
        public int ProjectId { get; set; }
        public string Name { get; set; }

        public int? DepartmentLeaderId { get; set; }

        public Employee? DepartmentLeader { get; set; }
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
