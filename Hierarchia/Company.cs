using System.ComponentModel.DataAnnotations.Schema;

namespace KrosOrg.Hierarchia
{
    public class Company
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Name { get; set; }
        public int? CEOID { get; set; }
        public Employee CEO { get; set; }
        public ICollection<Division> Divisions { get; set; } = new List<Division>();
    }
}
