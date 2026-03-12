using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Web_API.Utils;

namespace Web_API.Controllers
{
    [ApiController]
    [Route("api/clinics")]
    public class ClinicsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ClinicsController(AppDbContext context) 
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetClinics()
        {
            try
            {
                var clinics = await _context.Clinics
                                .Select(c => new ClinicTableDTO
                                {
                                    Id = c.Id,
                                    Location = c.Location,
                                    PostalCode = c.PostalCode,
                                    PhoneNumber = c.PhoneNumber,
                                    EmployeesCount = c.EmployeesCount
                                })
                                .ToListAsync();

                return Ok(clinics);
            }
            catch
            {
                return BadRequest("Не удалось получить клиника");
            }
        }
        [HttpGet("addresses")]
        public async Task<IActionResult> GetClinicsAdresses()
        {
            try
            {
                var addresses = await _context.Clinics.Select(c => c.Location).ToListAsync();
                return Ok(addresses);
            }
            catch
            {
                return BadRequest("Не удалось получить адреса клиник");
            }            
        }
        [HttpGet("services/{clinicId}")]
        public async Task<IActionResult> GetClinicsServices(int clinicId)
        {
            try
            {
                var services = await _context.Services
                    .Where(s => s.ClinicId == clinicId)
                    .Select(s => new ServiceDTO
                    {
                        Name = s.Name,
                        Description = s.Description,
                        BasePrice = s.BasePrice,
                        CategoryName = s.CategoryName,
                    })
                    .ToListAsync();
                return Ok(services);
            }
            catch
            {
                return BadRequest("Не удалось получить услуги клиники");
            }            
        }
    }
}
