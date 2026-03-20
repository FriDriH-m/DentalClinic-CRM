using Microsoft.AspNetCore.Mvc;
using Web_API.Models;
using Web_API.Utils;

namespace Web_API.Controllers
{
    [ApiController]
    [Route("api/clients")]
    public class ClientsController : Controller
    {
        private readonly AppDbContext _context;
        public ClientsController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet("bonuses")]
        public async Task<IActionResult> GetBonuses(int clientId)
        {
            try
            {

                return Ok();
            }
            catch { return BadRequest(); }
        }
        [HttpGet("{clientId}/bonuses")]
        public async Task<IActionResult> GetBonuses(int clientId)
        {
            try
            {
                
                return Ok();
            }
            catch { return BadRequest(); }
        }
        [HttpGet]
        public IActionResult GetAllClients()
        {
            var allClients = _context.Clients
                .Select(c => new ClientDTO 
                { 
                    Id = c.Id,
                    Status = c.Status,
                    MoneySpent = c.MoneySpent,
                    FirstName = c.FirstName,
                    SecondName = c.SecondName,
                    Info = c.Info,
                    Email = c.Email,
                    PhoneNumber = c.PhoneNumber,
                }).ToList();
            return Ok(allClients);
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterClientsAsync([FromBody] ClientDTO clientDTO)
        {
            try
            {
                Client newClient = new Client
                {
                    FirstName = clientDTO.FirstName,
                    SecondName = clientDTO.SecondName,
                    Info = clientDTO.Info,
                    PhoneNumber = clientDTO.PhoneNumber,
                    Email = clientDTO.Email
                };
                await _context.Clients.AddAsync(newClient);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
