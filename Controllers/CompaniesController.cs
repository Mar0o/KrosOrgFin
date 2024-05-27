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
    public class CompaniesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CompaniesController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Company>>> GetCompanies()
        {
            return await _context.Companies.ToListAsync();
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Company>> GetCompany(int id)
        {
            var company = await _context.Companies.FindAsync(id);

            if (company == null)
            {
                return NotFound();
            }

            return company;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompany(int id, Company company)
        {
            if (id != company.Id)
            {
                return BadRequest();
            }

            _context.Entry(company).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyExists(id))
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
        public async Task<ActionResult<Company>> PostCompany(DTO.CompanyDTO companyDto)
        {
            if (companyDto.CEO == null)
            {
                return BadRequest("CEO details are required.");
            }

            var ceo = new Employee
            {
                Name = companyDto.CEO.Name,
                BirthNumber = companyDto.CEO.BirthNumber,
                Email = companyDto.CEO.Email,
                Title = companyDto.CEO.Title,
                Phone = companyDto.CEO.Phone,
            };

            _context.Employees.Add(ceo);
            await _context.SaveChangesAsync();

            var company = new Company
            {
                Name = companyDto.Name,
                CEO = ceo,
                CEOID = ceo.Id 
            };

            _context.Companies.Add(company);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CompanyExists(company.Id))
                {
                    return Conflict(new { message = "Company with this ID already exists." });
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCompany", new { id = company.Id }, company);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }

            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CompanyExists(int id)
        {
            return _context.Companies.Any(e => e.Id == id);
        }
        [HttpGet("{id}/Divisions")]
        public async Task<ActionResult<IEnumerable<Division>>> GetCompanyDivisions(int id)
        {
            var company = await _context.Companies
                .Include(c => c.Divisions)
                .ThenInclude(d => d.Projects)
                .ThenInclude(p => p.Departments)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (company == null)
            {
                return NotFound();
            }

            return Ok(company.Divisions);
        }

        [HttpGet("{id}/Employees")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetCompanyEmployees(int id)
        {
            var company = await _context.Companies
                .Include(c => c.Divisions)
                .ThenInclude(di => di.Projects)
                .ThenInclude(p => p.Departments)
                .ThenInclude(d => d.Employees)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (company == null)
            {
                return NotFound();
            }

            var employees = company.Divisions
                .SelectMany(di => di.Projects)
                .SelectMany(p => p.Departments)
                .SelectMany(d => d.Employees)
                .Distinct()
                .ToList();

            return Ok(employees);
        }

        [HttpPost("AssignDivision")]
        public async Task<IActionResult> AssignDivisionToCompany(AssignDivisionToCompanyDTO dto)
        {
            var company = await _context.Companies.Include(c => c.Divisions).FirstOrDefaultAsync(c => c.Id == dto.CompanyId);
            if (company == null)
            {
                return NotFound("Company not found.");
            }
            var divisions = await _context.Divisions.Where(d => dto.DivisionIds.Contains(d.Id)).ToListAsync();
            if (divisions.Count != dto.DivisionIds.Count)
            {
                return BadRequest("One or more Divisions not found.");
            }
            foreach (var division in divisions)
            {
                if (!company.Divisions.Contains(division))
                {
                    company.Divisions.Add(division);
                }
            }
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

