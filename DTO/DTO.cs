using Microsoft.CodeAnalysis.Options;

namespace KrosOrg.DTO
{
    public class CompanyDTO

    {
        public string Name { get; set; }
        public int Id { get; set; }
        public EmployeeDto CEO { get; set; }

    }

    public class EmployeeDto
    {
        public int Id { get; set; }
        public string BirthNumber { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public string Phone { get; set; }
    }
    public class DivisionDTO
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public DivisionLeaderDTO DivisionLeader { get; set; }
        public CompanyDTO Company { get; set; }
    }

    public class DivisionLeaderDTO
    {
        public int Id { get; set; }
        public string BirthNumber { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public string Phone { get; set; }

    }

    public class ProjectDTO {
        public string Name { get; set; }
        public int Id { get; set; }
        public ProjectLeaderDTO ProjectLeader { get; set; }
        public DivisionDTO Division { get; set; }
    }
    public class ProjectLeaderDTO {
        public int Id { get; set; }
        public string BirthNumber { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public string Phone { get; set; }
    }
    public class DepartmentDTO {
        public string Name { get; set; }
        public int Id { get; set; }
        public DepartmentLeaderDTO DepartmentLeaderDto { get; set; }
        public ProjectDTO Project { get; set; }
    }
    public class DepartmentLeaderDTO { 
        public int Id { get; set; }
        public string BirthNumber { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public string Phone { get; set; }
    }
    public class AssignEmployeesToDepartmentDTO
    {
        public int DepartmentId { get; set; }
        public List<int> EmployeeIds { get; set; }
    }
    public class AssignDepartmentsToProjectDTO
    {
        public int ProjectId { get; set; }
        public List<int> DepartmentIds { get; set; }
    
    }
    public class AssignProjectToDivisionDTO {
        public int DivisionId { get; set; }
        public List<int> ProjectIds { get; set; }
    }
    public class AssignDivisionToCompanyDTO {
        public int CompanyId { get; set; }
        public List<int> DivisionIds { get; set; }
    }
}
