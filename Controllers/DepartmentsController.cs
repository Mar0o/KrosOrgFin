using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KrosOrg.Data;
using KrosOrg.Hierarchia;
using KrosOrg.DTO;

namespace KrosOrg.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DepartmentsController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartment()
        {
            return await _context.Departments.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Department>> GetDepartment(int id)
        {
            var department = await _context.Departments.FindAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            return department;
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutDepartment(int id, Department department)
        {
            if (id != department.Id)
            {
                return BadRequest();
            }

            _context.Entry(department).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        [HttpPost]
        public async Task<ActionResult<Department>> PostProject(DepartmentDTO departmentDTO)
        {
            if (departmentDTO == null)
            {
                return BadRequest("Department data is null.");
            }


            var department = new Department
            {
                Id = departmentDTO.Id,
                Name = departmentDTO.Name,
                ProjectId = departmentDTO.Project?.Id ?? 0,
                DepartmentLeaderId = departmentDTO.DepartmentLeaderDto?.Id
            };


            var project = await _context.Projects.FindAsync(departmentDTO.Project?.Id);
            if (project == null)
            {
                return BadRequest("Invalid Department ID.");
            }
            department.Project = project;


            if (departmentDTO.DepartmentLeaderDto != null)
            {
                var DepartmentLeader = await _context.Employees.FindAsync(departmentDTO.DepartmentLeaderDto.Id);
                if (DepartmentLeader == null)
                {
                    return BadRequest("Invalid Department Leader ID.");
                }
                department.DepartmentLeader = DepartmentLeader;
            }

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDepartment", new { id = department.Id }, department);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(e => e.Id == id);
        }

        [HttpPost("AssignEmployees")]
        public async Task<IActionResult> AssignEmployeesToDepartment(AssignEmployeesToDepartmentDTO dto)
        {
            var department = await _context.Departments.Include(d => d.Employees).FirstOrDefaultAsync(d => d.Id == dto.DepartmentId);
            if (department == null)
            {
                return NotFound("Department not found.");
            }

            var employees = await _context.Employees.Where(e => dto.EmployeeIds.Contains(e.Id)).ToListAsync();
            if (employees.Count != dto.EmployeeIds.Count)
            {
                return BadRequest("One or more employees not found.");
            }

            foreach (var employee in employees)
            {
                if (!department.Employees.Contains(employee))
                {
                    department.Employees.Add(employee);
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("{id}/Employees")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetDepartmentEmployees(int id)
        {
            var department = await _context.Departments
                .Include(d => d.Employees)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (department == null)
            {
                return NotFound();
            }

            return Ok(department.Employees);
        }
    }
}
