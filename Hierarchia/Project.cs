using System.ComponentModel.DataAnnotations.Schema;

namespace KrosOrg.Hierarchia
{
    public class Project
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public int DivisionId { get; set; }
        public Division Division { get; set; }
        public string Name { get; set; }
        public int? ProjectLeaderId { get; set; }
        public Employee? ProjectLeader { get; set; }
        public ICollection<Department> Departments { get; set; } = new List<Department>();

    }
}
