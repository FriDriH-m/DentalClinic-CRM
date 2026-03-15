using Microsoft.AspNetCore.Mvc;
using Web_API.Utils;

namespace Web_API.Controllers
{
    [ApiController]
    [Route("api/materials")]
    public class MaterialsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public MaterialsController(AppDbContext context) 
        {
            _context = context;
        }
        [HttpPost]
        public IActionResult AddNewMaterial([FromBody] MaterialDTO material)
        {

            return Ok();
        }
        [HttpGet]
        public IActionResult GetAllMaterials()
        {
            try
            {
                var materials = _context.Materials
                    .Select(m => new MaterialDTO
                    {
                        Id = m.Id,
                        Name = m.Name,
                        Description = m.Description,
                        IsCertifiedMaterial = m.IsCertifiedMaterial,
                        Price = m.Price,
                        PurchasePrice = m.PurchasePrice,
                        Count = m.Count,
                        ClinicId = m.ClinicId
                    })
                    .ToList();

                return Ok(materials);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("{clinicId}")]
        public IActionResult GetClinicsMaterials(int clinicId)
        {
            try
            {
                var materials = _context.Materials
                    .Where(m => m.ClinicId == clinicId)
                    .Select(m => new MaterialDTO
                    {
                        Id = m.Id,
                        Name = m.Name,
                        Description = m.Description,
                        IsCertifiedMaterial = m.IsCertifiedMaterial,
                        Price = m.Price,
                        PurchasePrice = m.PurchasePrice,
                        Count = m.Count,
                        ClinicId = m.ClinicId
                    })
                    .ToList();

                return Ok(materials);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }            
        }
    }
}
