using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        [HttpGet("bonuses")]
        public async Task<IActionResult> GetBonuses()
        {
            try
            {
                var bonuses = await _context.Bonuses
                    .Where(b => b.ExpiredAt > DateTime.UtcNow)
                    .GroupBy(b => b.ClientId)
                    .Select(b => new BonuseDTO 
                    {
                        ClientId = b.Key,
                        Amount = b.Sum(g => g.Amount)
                    })
                    .ToDictionaryAsync
                    (
                        dto => dto.ClientId,  
                        dto => dto.Amount
                    );

                return Ok(bonuses);
            }
            catch { return BadRequest(); }
        }
        [HttpGet("{clientId}/bonuses")]
        public async Task<IActionResult> GetBonuses(int clientId)
        {
            try
            {
                var bonuses = _context.Bonuses.Where(b => b.ClientId == clientId)
                    .Select(b => new BonuseDTO
                    {
                        ClientId = b.ClientId,
                        Amount = b.Amount
                    }).ToList();
                return Ok(bonuses);
            }
            catch { return BadRequest(); }
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
        [HttpPut]
        public async Task<IActionResult> UpdateClientInfo([FromBody] ClientDTO clientDTO)
        {
            try
            {
                var client = await _context.Clients.FindAsync(clientDTO.Id);
                if (client == null) return NotFound("Пользователь не найден");

                client.FirstName = clientDTO.FirstName;
                client.SecondName = clientDTO.SecondName;
                client.Info = clientDTO.Info;
                client.PhoneNumber = clientDTO.PhoneNumber;
                client.Email = clientDTO.Email;

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
