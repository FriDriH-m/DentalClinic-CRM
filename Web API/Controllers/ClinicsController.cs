using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

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
        [HttpGet("addresses")]
        public async Task<IActionResult> GetClinicsAdresses()
        {
            var addresses = await _context.Clinics.Select(c => c.Location).ToListAsync();
            return Ok(addresses);
        } 
    }
}
