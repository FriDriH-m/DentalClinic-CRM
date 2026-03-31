using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_API.Models;
using Web_API.Utils;

namespace Web_API.Controllers
{
    [ApiController]
    [Route("api/appointments")]
    public class AppointmentsController : Controller
    {
        private readonly AppDbContext _context;
        public AppointmentsController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet("{clientId}")]
        public async Task<IActionResult> GetClientAppointments(int clientId)
        {
            return await FindAppointments(clientId);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAppointments()
        {
            return await FindAppointments(0);
        }
        [HttpGet("checks")]
        public async Task<IActionResult> GetChecks()
        {
            try
            {
                var checks = await _context.Checks
                    .Select(c => new CheckDTO
                    {
                        Id = c.Id,
                        Date = c.Date,
                        TotalPrice = c.TotalPrice,
                        Discount = c.Discount,
                        Appointment = new AppointmentDTO
                        {
                            Id = c.Appointment.Id,
                            Date = c.Appointment.Date,
                            EndTime = c.Appointment.EndTime,
                            Status = c.Appointment.Status,
                            TotalPrice = c.Appointment.TotalPrice,
                            Discount = c.Appointment.Discount,
                            IsClosed = c.Appointment.IsClosed,
                            ClientId = c.Appointment.ClientId,
                            ClinicId = c.Appointment.ClinicId,
                            EmployeeId = c.Appointment.EmployeeId
                        }
                    })
                    .ToListAsync();

                return Ok(checks);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("free_doctors")]
        public async Task<IActionResult> GetFreeDoctorsAsync([FromQuery] DateTime datetime, [FromQuery] int serviceId)
        {
            bool needCertifiedDoctor = false;
            var service = await _context.Services.FindAsync(serviceId);
            if (service == null) return BadRequest("Услуга не найдена");

            var serviceMaterial = await _context.ServiceMaterials
                .Include(m => m.Material)
                .Where(m => m.ServiceId == serviceId)
                .ToListAsync(); 

            foreach (var material in serviceMaterial)
            {
                if (material.Material.IsCertifiedMaterial)
                {
                    needCertifiedDoctor = true;
                }
            }

            var doctors = await _context.DoctorCategorySkills
                .Include(dcs => dcs.Employee)
                    .ThenInclude(e => e.Appointments)
                .Select(dcs => dcs.Employee)
                .Distinct()
                .ToListAsync(); 

            int duration = service.DurationMinutes; 
            var appointmentEnd = datetime.AddMinutes(duration);

            var freeDoctors = doctors
                .Where(e => !e.Appointments.Any(a =>
                    a.Date.Date == datetime.Date &&  // Тот же день
                    a.Date < appointmentEnd &&        // Пересечение
                    a.EndTime > datetime
                ))
                .ToList();

            var doctorsSkills = _context.DoctorCategorySkills
                .Where(c => c.CategoryId == service.CategoryId)
                .Select(d => d.EmployeeId)
                .ToList();

            var skilledFreeDoctors = freeDoctors
                .Where(d => doctorsSkills.Contains(d.Id));

            var result = skilledFreeDoctors.Select(e => new EmployeeTableDTO
            {
                Id = e.Id,
                FirstName = e.FirstName,
                SecondName = e.SecondName,
                PhoneNumber = e.PhoneNumber,
                Specialization = e.Specialization,
                Info = e.Info,
                IsCertified = e.IsCertified ?? false,
                Age = e.Age,
                Salary = e.Salary,
                Experience = e.Experience,
                DbUsername = e.DbUsername
            }).ToList();

            if (needCertifiedDoctor)
            {
                result = result.Where(d => d.IsCertified == true).ToList();
            }

            return Ok(result);
        }
        [HttpGet("materials/{appointmentId}")]
        public async Task<IActionResult> GetAppointmentsMaterials(int appointmentId)
        {
            try
            {
                var materials = await _context.AppointmentMaterial
                    .Include(am => am.Material)
                    .Where(am => am.AppointmentId == appointmentId)
                    .Select(m => new MaterialDTO
                    {
                        Id = m.Material.Id,
                        Name = m.Material.Name,
                        Description = m.Material.Description,
                        IsCertifiedMaterial = m.Material.IsCertifiedMaterial,
                        PurchasePrice = m.Material.PurchasePrice,
                        Price = m.Material.Price,
                        Count = m.Quantity,
                        ClinicId = m.Material.ClinicId
                    })
                    .ToListAsync();

                return Ok(materials);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> RegisterAppointment([FromBody] AppointmentDTO appointment)
        {
            try
            {
                Appointment newAppointment = new Appointment
                {
                    Date = appointment.Date,
                    EndTime = appointment.EndTime,
                    TotalPrice = appointment.TotalPrice,
                    Discount = appointment.Discount,
                    IsClosed = false,
                    ClientId = appointment.ClientId,
                    ClinicId = appointment.ClinicId,
                    EmployeeId = appointment.EmployeeId
                };
                newAppointment.Date = DateTime.SpecifyKind(newAppointment.Date, DateTimeKind.Utc);
                newAppointment.EndTime = DateTime.SpecifyKind(newAppointment.EndTime, DateTimeKind.Utc);
                await _context.Appointments.AddAsync(newAppointment);
                await _context.SaveChangesAsync();

                AppointmentService appointmentService = new AppointmentService
                {
                    AppointmentId = newAppointment.Id,
                    ServiceId = appointment.ServiceId
                };
                await _context.AppointmentService.AddAsync(appointmentService);

                var materials = _context.ServiceMaterials
                    .Include(s => s.Material)
                    .Where(s => s.ServiceId == appointment.ServiceId)
                    .ToList();

                AppointmentMaterial appointmentMaterial;
                foreach (var material in materials)
                {
                    appointmentMaterial = new AppointmentMaterial
                    {
                        AppointmentId = newAppointment.Id,
                        MaterialId = material.MaterialId,
                        Quantity = material.Count,
                        Price = material.Material.Price
                    };
                    await _context.AppointmentMaterial.AddAsync(appointmentMaterial);
                }
                await _context.SaveChangesAsync();


                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("materials")]
        public async Task<IActionResult> ChangeAppointmentMaterials([FromBody] AppointmentMaterialsChange appointmentChanges)
        {
            try
            {
                var oldLinks = await _context.AppointmentMaterial
                    .Where(m => m.AppointmentId == appointmentChanges.appointmentDTO.Id)
                    .ToListAsync();

                _context.AppointmentMaterial.RemoveRange(oldLinks);

                if (appointmentChanges.appointmentDTO.MaterialsId != null && appointmentChanges.appointmentDTO.MaterialsId.Count > 0)
                {
                    foreach (var materialId in appointmentChanges.appointmentDTO.MaterialsId)
                    {
                        var materialExists = await _context.Materials.FindAsync(materialId.Key);
                        if (materialExists == null)
                        {
                            return BadRequest($"Материал с Id {materialId} не найден");
                        }

                        await _context.AppointmentMaterial.AddAsync(new AppointmentMaterial
                        {
                            AppointmentId = appointmentChanges.appointmentDTO.Id,
                            MaterialId = materialId.Key,
                            Quantity = materialId.Value,
                            Price = materialExists.Price
                        });
                    }
                }

                var appointment = await _context.Appointments.FindAsync(appointmentChanges.appointmentDTO.Id);

                appointment.TotalPrice += appointmentChanges.priceChange;

                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPatch("{appointmentId}/{status}")]
        public async Task<IActionResult> CloseAppointment(int appointmentId, AppointmentStatus status)
        {
            try
            {
                var appointment = await _context.Appointments.FindAsync(appointmentId);
                appointment.Status = status;

                await _context.SaveChangesAsync();
                return Ok();
            }
            catch
            {
                return BadRequest("Не удалось закрыть запись");
            }
        }        
        private async Task<IActionResult> FindAppointments(int clientId)
        {
            try
            {
                var appointmentsQuery = _context.Appointments.AsQueryable();

                if (clientId != 0)
                {
                    appointmentsQuery = appointmentsQuery.Where(a => a.ClientId == clientId);
                }
                
                var appointmentsList = await appointmentsQuery
                    .Select(a => new AppointmentDTO
                    {
                        Id = a.Id,
                        Date = a.Date,
                        EndTime = a.EndTime,
                        Status = a.Status,
                        TotalPrice = a.TotalPrice,
                        Discount = a.Discount,
                        IsClosed = a.IsClosed,
                        ClientId = a.ClientId,
                        ClinicId = a.ClinicId,
                        ServiceId = _context.AppointmentService
                                        .Where(s => s.AppointmentId == a.Id)
                                        .Select(s => s.ServiceId)
                                        .FirstOrDefault(),
                        EmployeeId = a.EmployeeId,
                    })
                    .ToListAsync();

                return Ok(appointmentsList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
