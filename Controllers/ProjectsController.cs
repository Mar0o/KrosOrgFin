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
    public class ProjectsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProjectsController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProject()
        {
            return await _context.Projects.ToListAsync();
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            return project;
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutProject(int id, Project project)
        {
            if (id != project.Id)
            {
                return BadRequest();
            }

            _context.Entry(project).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id))
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
        public async Task<ActionResult<Project>> PostProject(ProjectDTO projectDTO)
        {
            if (projectDTO == null)
            {
                return BadRequest("Project data is null.");
            }


            var project = new Project
            {
                Id = projectDTO.Id,
                Name = projectDTO.Name,
                DivisionId = projectDTO.Division?.Id ?? 0,
                ProjectLeaderId = projectDTO.ProjectLeader?.Id
            };

            var division = await _context.Divisions.FindAsync(projectDTO.Division?.Id);
            if (division == null)
            {
                return BadRequest("Invalid Project ID.");
            }
            project.Division = division;

            if (projectDTO.ProjectLeader != null)
            {
                var ProjectLeader = await _context.Employees.FindAsync(projectDTO.ProjectLeader.Id);
                if (ProjectLeader == null)
                {
                    return BadRequest("Invalid Project Leader ID.");
                }
                project.ProjectLeader = ProjectLeader;
            }

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProject", new { id = project.Id }, project);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }
        [HttpPost("AssignDepartment")]
        public async Task<IActionResult> AssignDepartmentToProject(AssignDepartmentsToProjectDTO dto)
        {
            var project = await _context.Projects.Include(p => p.Departments).FirstOrDefaultAsync(p => p.Id == dto.ProjectId);
            if (project == null)
            {
                return NotFound("Project not found.");
            }

            var departments = await _context.Departments.Where(d => dto.DepartmentIds.Contains(d.Id)).ToListAsync();
            if (departments.Count != dto.DepartmentIds.Count)
            {
                return BadRequest("One or more Departments not found.");
            }

            foreach (var department in departments)
            {
                if (!project.Departments.Contains(department))
                {
                    project.Departments.Add(department);
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }


        [HttpGet("{id}/Departments")]
        public async Task<ActionResult<IEnumerable<Department>>> GetProjectDepartments(int id)
        {
            var project = await _context.Projects
                .Include(p => p.Departments)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            return Ok(project.Departments);
        }

        [HttpGet("{id}/Employees")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetProjectEmployees(int id)
        {
            var project = await _context.Projects
                .Include(p => p.Departments)
                .ThenInclude(d => d.Employees)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            var employees = project.Departments
                .SelectMany(d => d.Employees)
                .Distinct()
                .ToList();

            return Ok(employees);
        }
    }
}
