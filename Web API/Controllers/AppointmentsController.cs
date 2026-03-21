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
        [HttpPost]
        public async Task<IActionResult> RegisterAppointment([FromBody] AppointmentDTO appointment)
        {
            try
            {
                Appointment newAppointment = new Appointment
                {
                    Date = appointment.Date,
                    EndTime = appointment.EndTime,
                    Status = appointment.Status,
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
            catch
            {
                return BadRequest();
            }
        }
        [HttpGet("free_doctors")]
        public async Task<IActionResult> GetFreeDoctorsAsync([FromQuery] DateTime datetime, [FromQuery] int serviceId)
        {
            var service = await _context.Services.FindAsync(serviceId);
            if (service == null) return BadRequest("Услуга не найдена");

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

            var result = freeDoctors.Select(e => new EmployeeTableDTO
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

            return Ok(result);
        }
    }
}
