using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        [HttpGet("free_doctors")]
        public async Task<IActionResult> IndexAsync([FromQuery] DateTime datetime, [FromQuery] int serviceId)
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
