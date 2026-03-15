using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Web_API.Models;
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
        [HttpDelete("{clinicId}")]
        public async Task<IActionResult> DeleteClinic(int clinicId)
        {
            try
            {
                await _context.Clinics.Where(c => c.Id == clinicId).ExecuteDeleteAsync();

                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (DbUpdateException ex)
            {
                // Ошибка при сохранении в БД (нарушение constraints, и т.д.)
                var innerMessage = ex.InnerException?.Message ?? ex.Message;
                return BadRequest($"Database error: {innerMessage}");
            }
            catch (Npgsql.PostgresException ex)
            {
                // Специфичная ошибка PostgreSQL (permission denied, duplicate key, etc.)
                return BadRequest($"PostgreSQL error: {ex.MessageText} (Code: {ex.SqlState})");
            }
            catch (Exception ex)
            {
                // Непредвиденная ошибка
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }
        [HttpPost]
        public async Task<IActionResult> RegisterClinic([FromBody] ClinicTableDTO clinic)
        {
            try
            {
                Clinic newClinic = new Clinic
                {
                    Location = clinic.Location,
                    PostalCode = clinic.PostalCode,
                    PhoneNumber = clinic.PhoneNumber,
                    EmployeesCount = 0
                };
                await _context.Clinics.AddAsync(newClinic);

                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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
        [HttpGet("services")]
        public async Task<IActionResult> GetAllServices(int clinicId)
        {
            try
            {               
                var services = await _context.Services
                    .Include(c => c.Clinic)
                    .Select(s => new ServiceDTO
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Description = s.Description,
                        DurationMinutes = s.DurationMinutes,
                        BasePrice = s.BasePrice,
                        CategoryName = s.CategoryName,
                        ClinicId = s.ClinicId,
                        ClinicAddress = s.Clinic.Location
                    })
                    .ToListAsync();
                return Ok(services);
            }
            catch
            {
                return BadRequest("Не удалось получить услуги клиник");
            }
        }
        [HttpPost("services")]
        public async Task<IActionResult> AddNewService([FromBody] RegisterService service)
        {
            try
            {
                ServiceDTO serviceDTO = service.service;
                Service newService = new Service
                {
                    Name = serviceDTO.Name,
                    Description = serviceDTO.Description,
                    DurationMinutes = serviceDTO.DurationMinutes,
                    BasePrice = serviceDTO.BasePrice,
                    CategoryName = serviceDTO.CategoryName,
                    CategoryId = serviceDTO.CategoryId,
                    ClinicId = serviceDTO.ClinicId,
                };

                await _context.Services.AddAsync(newService);
                await _context.SaveChangesAsync();

                for (int i = 0; i < service.materialsId.Length; i++)
                {
                    ServiceMaterials link = new ServiceMaterials
                    {
                        MaterialId = service.materialsId[i],
                        ServiceId = newService.Id
                    };
                    await _context.ServiceMaterials.AddAsync(link);
                }
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка сервера: {ex.Message}");
            }
        }
        [HttpDelete("services/{serviceId}")]
        public async Task<IActionResult> DeleteService(int serviceId)
        {
            try
            {
                await _context.Services
                    .Where(s => s.Id == serviceId)
                    .ExecuteDeleteAsync();
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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
