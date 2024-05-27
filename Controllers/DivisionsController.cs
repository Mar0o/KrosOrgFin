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
    public class DivisionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DivisionsController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Division>>> GetDivisions()
        {
            return await _context.Divisions.ToListAsync();
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Division>> GetDivision(int id)
        {
            var division = await _context.Divisions
                .Include(d => d.Company)
                .Include(d => d.DivisionLeader)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (division == null)
            {
                return NotFound();
            }

            return division;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutDivision(int id, Division division)
        {
            if (id != division.Id)
            {
                return BadRequest();
            }

            _context.Entry(division).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DivisionExists(id))
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
        public async Task<ActionResult<Division>> PostDivision(DivisionDTO divisionDto)
        {
            if (divisionDto == null)
            {
                return BadRequest("Division data is null.");
            }

            var division = new Division
            {
                Id = divisionDto.Id,
                Name = divisionDto.Name,
                CompanyId = divisionDto.Company?.Id ?? 0,
                DivisionLeaderId = divisionDto.DivisionLeader?.Id
            };


            var company = await _context.Companies.FindAsync(divisionDto.Company?.Id);
            if (company == null)
            {
                return BadRequest("Invalid Company ID.");
            }
            division.Company = company;

            if (divisionDto.DivisionLeader != null)
            {
                var divisionLeader = await _context.Employees.FindAsync(divisionDto.DivisionLeader.Id);
                if (divisionLeader == null)
                {
                    return BadRequest("Invalid Division Leader ID.");
                }
                division.DivisionLeader = divisionLeader;
            }

            _context.Divisions.Add(division);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDivision", new { id = division.Id }, division);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDivision(int id)
        {
            var division = await _context.Divisions.FindAsync(id);
            if (division == null)
            {
                return NotFound();
            }

            _context.Divisions.Remove(division);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DivisionExists(int id)
        {
            return _context.Divisions.Any(e => e.Id == id);
        }

        [HttpGet("{id}/Departments")]
        public async Task<ActionResult<IEnumerable<Project>>> GetDivisionProjects(int id)
        {
            var division = await _context.Divisions
                .Include(d => d.Projects)
                .ThenInclude(p => p.Departments)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (division == null)
            {
                return NotFound();
            }

            return Ok(division.Projects);
        }

        [HttpGet("{id}/Employees")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetDivisionEmployees(int id)
        {
            var division = await _context.Divisions
                .Include(di => di.Projects)
                .ThenInclude(p => p.Departments)
                .ThenInclude(d => d.Employees)
                .FirstOrDefaultAsync(di => di.Id == id);

            if (division == null)
            {
                return NotFound();
            }

            var employees = division.Projects
                .SelectMany(p => p.Departments)
                .SelectMany(d => d.Employees)
                .Distinct()
                .ToList();

            return Ok(employees);
        }

        [HttpPost("AssignProject")]
        public async Task<IActionResult> AssignProjectToDivision(AssignProjectToDivisionDTO dto)
        {
            var division = await _context.Divisions.Include(d => d.Projects).FirstOrDefaultAsync(d => d.Id == dto.DivisionId);
            if (division == null)
            {
                return NotFound("Division not found.");
            }

            var projects = await _context.Projects.Where(p => dto.ProjectIds.Contains(p.Id)).ToListAsync();
            if (projects.Count != dto.ProjectIds.Count)
            {
                return BadRequest("One or more Projects not found.");
            }

            foreach (var project in projects)
            {
                if (!division.Projects.Contains(project))
                {
                    division.Projects.Add(project);
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
