using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Web_API.Models;
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
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMaterial(int id, [FromBody] MaterialDTO materialDTO)
        {
            try
            {
                if (id != materialDTO.Id) return BadRequest("ID не совпадает");

                var material = await _context.Materials.FindAsync(id);
                if (material == null) return NotFound();

                material.Name = materialDTO.Name;
                material.Description = materialDTO.Description;
                material.Count = materialDTO.Count;
                material.Price = materialDTO.Price;
                material.PurchasePrice = materialDTO.PurchasePrice;
                material.IsCertifiedMaterial = materialDTO.IsCertifiedMaterial;

                await _context.SaveChangesAsync();
                return Ok(material);
            }
            catch 
            {
                return BadRequest();
            }
            
        }
        [HttpPost]
        public async Task<IActionResult> AddNewMaterial([FromBody] MaterialDTO material)
        {
            Material newMaterial = new Material
            {
                Name = material.Name,
                Description = material.Description,
                Price = material.Price,
                PurchasePrice = material.PurchasePrice,
                Count = material.Count,
                IsCertifiedMaterial = material.IsCertifiedMaterial,
                ClinicId = material.ClinicId,
            };

            await _context.Materials.AddAsync(newMaterial);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpGet]
        public IActionResult GetAllMaterials()
        {
            try
            {
                var materials = _context.Materials
                    .Include(m => m.Clinic)
                    .Select(m => new MaterialDTO
                    {
                        Id = m.Id,
                        Name = m.Name,
                        Description = m.Description,
                        IsCertifiedMaterial = m.IsCertifiedMaterial,
                        Price = m.Price,
                        PurchasePrice = m.PurchasePrice,
                        Count = m.Count,
                        ClinicId = m.ClinicId,
                        ClinicAddress = m.Clinic.Location
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
                var materials =  _context.Materials
                    .Where(m => m.ClinicId == clinicId)
                    .Include(m => m.Clinic)
                    .Select(m => new MaterialDTO
                    {
                        Id = m.Id,
                        Name = m.Name,
                        Description = m.Description,
                        IsCertifiedMaterial = m.IsCertifiedMaterial,
                        Price = m.Price,
                        PurchasePrice = m.PurchasePrice,
                        Count = m.Count,
                        ClinicId = m.ClinicId,
                        ClinicAddress = m.Clinic.Location
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
