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
        public async Task<IActionResult> GetAllServices(int? clinicId)
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
        [HttpPut("services/{id}")]
        public async Task<IActionResult> UpdateService(int id, [FromBody] RegisterService service)
        {
            try
            {
                var existingService = await _context.Services.FindAsync(id);
                if (existingService == null)
                {
                    return NotFound("Услуга не найдена");
                }

                existingService.Name = service.service.Name;
                existingService.Description = service.service.Description;
                existingService.DurationMinutes = service.service.DurationMinutes;
                existingService.BasePrice = service.service.BasePrice;
                existingService.CategoryName = service.service.CategoryName;
                existingService.CategoryId = service.service.CategoryId;
                existingService.ClinicId = service.service.ClinicId;

                var oldLinks = await _context.ServiceMaterials
                    .Where(sm => sm.ServiceId == id)
                    .ToListAsync();
                _context.ServiceMaterials.RemoveRange(oldLinks);

                if (service.materialsId != null && service.materialsId.Count > 0)
                {
                    foreach (var materialId in service.materialsId)
                    {
                        var materialExists = await _context.Materials.AnyAsync(m => m.Id == materialId.Key);
                        if (!materialExists)
                        {
                            return BadRequest($"Материал с Id {materialId} не найден");
                        }

                        await _context.ServiceMaterials.AddAsync(new ServiceMaterials
                        {
                            ServiceId = id,
                            MaterialId = materialId.Key,
                            Count = materialId.Value
                        });
                    }
                }

                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка сервера: {ex.Message}");
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
                    ClinicId = serviceDTO.ClinicId
                };

                await _context.Services.AddAsync(newService);
                await _context.SaveChangesAsync();

                foreach (var material in service.materialsId)
                {
                    ServiceMaterials link = new ServiceMaterials
                    {
                        MaterialId = material.Key,
                        ServiceId = newService.Id,
                        Count = material.Value
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
        [HttpGet("services/{serviceId}/materials/availability")]
        public async Task<IActionResult> CheckMaterialsAvailability(int serviceId, int clinicId)
        {
            var serviceMaterials = await _context.ServiceMaterials
                .Include(sm => sm.Material)
                .Where(sm => sm.ServiceId == serviceId && sm.Material.ClinicId == clinicId)
                .ToListAsync();

            if (serviceMaterials.Count == 0)
            {
                return Ok();
            }                

            var availability = serviceMaterials.Select(sm => new 
            {
                IsAvailable = sm.Material.Count >= sm.Count,    
            }).ToList();

            bool allAvailable = availability.All(a => a.IsAvailable);

            if (allAvailable)
            {
                return Ok();
            }
            else
            {
                return BadRequest("Недостаточно материалов на складе");
            }
        }
        [HttpGet("services/{id}/materials")]
        public async Task<IActionResult> GetServiceMaterials(int id)
        {
            try
            {
                var serviceMaterials = await _context.ServiceMaterials
                    .Include(sm => sm.Material)
                    .Where(sm => sm.ServiceId == id)
                    .Select(m => new MaterialDTO
                    {
                        Id = m.Material.Id,
                        Name = m.Material.Name,
                        Description = m.Material.Description,
                        IsCertifiedMaterial = m.Material.IsCertifiedMaterial,
                        PurchasePrice = m.Material.PurchasePrice,
                        Price = m.Material.Price,
                        Count = m.Count,
                        ClinicId = m.Material.ClinicId
                    })
                    .ToListAsync();

                return Ok(serviceMaterials);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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
                        Id = s.Id,
                        Name = s.Name,
                        Description = s.Description,
                        DurationMinutes = s.DurationMinutes,
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
