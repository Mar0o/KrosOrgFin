using System.ComponentModel.DataAnnotations.Schema;

namespace KrosOrg.Hierarchia
{
    public class Employee
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string BirthNumber { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public string Phone { get; set; }
    }
}
