using Dapper;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Threading.Tasks;
using Web_API.Models;
using Web_API.Utils;

namespace Web_API.Controllers
{
    [ApiController]
    [Route("api/employees")]
    public class EmployeesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public EmployeesController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        [HttpGet("doctors")]
        public async Task<IActionResult> GetAllDoctors()
        {
            try
            {
                var doctors = await _context.DoctorCategorySkills
                .Include(e => e.Employee)
                .Select(e => e.Employee)
                .Distinct()
                .Select(e => new EmployeeTableDTO
                {
                    Id = e.Id,
                    FirstName = e.FirstName,
                    SecondName = e.SecondName,
                    PhoneNumber = e.PhoneNumber,
                    Specialization = e.Specialization,
                    Info = e.Info,
                    Age = e.Age,
                    Salary = e.Salary,
                    Experience = e.Experience,
                    DbUsername = e.DbUsername
                }).ToListAsync();

                return Ok(doctors);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPatch("doctors/{employeeId}")]
        public async Task<IActionResult> GiveCertificate(int employeeId)
        {
            try
            {
                var doctor = await _context.Employees.FindAsync(employeeId);
                doctor.IsCertified = true;

                await _context.SaveChangesAsync();
                return Ok();
            }
            catch
            {
                return BadRequest();    
            }            
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            var employees =
                await _context.Employees
                            .Select(e => new EmployeeTableDTO
                            {
                                Id = e.Id,
                                FirstName = e.FirstName,
                                SecondName = e.SecondName,
                                PhoneNumber = e.PhoneNumber,
                                Specialization = e.Specialization,
                                Info = e.Info,
                                Age = e.Age,
                                Salary = e.Salary,
                                Experience = e.Experience,
                                DbUsername = e.DbUsername
                            })
                            .ToListAsync();
            return Ok(employees);
        }

        [HttpDelete("{username}")]
        public async Task<IActionResult> DeleteEmployee(string username)
        {
            try
            {
                var employee = await _context.Employees.FirstOrDefaultAsync(e => e.DbUsername == username);
                if (employee == null) return NotFound();

                await _context.ClinicEmployees
                    .Where(c => c.EmployeeId == employee.Id)
                    .ExecuteDeleteAsync();

                await _context.DoctorCategorySkills
                    .Where(e => e.EmployeeId == employee.Id)
                    .ExecuteDeleteAsync();

                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();

                string ownerConnString = _configuration.GetConnectionString("OwnerConnection");

                using (var conn = new NpgsqlConnection(ownerConnString))
                {
                    await conn.OpenAsync();

                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;

                        cmd.CommandText = $"DROP OWNED BY \"{username}\" CASCADE;";
                        await cmd.ExecuteNonQueryAsync();

                        cmd.CommandText = $"DROP USER IF EXISTS \"{username}\";";
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при удалении: {ex.Message}");
            }

            return Ok();

        }
    }
}
