using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_API.Utils;

namespace Web_API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthorizeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthorizeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult CheckUser([FromBody] UserDTO userInfo)
        {

            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> GetSomething()
        {
            try
            {
                // ✅ Добавьте AS "Value" чтобы EF Core мог замаппить результат
                var serverTime = await _context.Database
                    .SqlQueryRaw<DateTime>("SELECT NOW() AS \"Value\"")
                    .FirstOrDefaultAsync();

                await Task.Delay(50);

                return Ok(new
                {
                    status = "OK",
                    timestamp = serverTime,
                    message = "Connection pool test successful"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, inner = ex.InnerException?.Message });
            }
        }
    }
}
