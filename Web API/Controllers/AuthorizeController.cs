using Dapper;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Threading.Tasks;
using Web_API.Models;
using Web_API.Utils;

namespace Web_API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthorizeController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthorizeController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginUserDTO userName)
        {
            (bool, string?) connectedEmployee = await TryConnectToDb(userName.Login, userName.Password);

            if (!connectedEmployee.Item1 || connectedEmployee.Item2 == null) return Unauthorized("Неверный логин или пароль");

            var employee = _context.Employees.FirstOrDefault(e => e.DbUsername == userName.Login);

            if (employee == null) return Unauthorized("Пользователт не найден, ошибка базы данных");


            return Ok(new SessionInfo(employee.Id, connectedEmployee.Item2));
        }

        private async Task<(bool, string?)> TryConnectToDb(string username, string password)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            var builder = new NpgsqlConnectionStringBuilder(connectionString)
            {
                Username = username,
                Password = password
            };

            try
            {
                await using var conn = new NpgsqlConnection(builder.ConnectionString);
                await conn.OpenAsync();

                var role = await conn.QueryFirstOrDefaultAsync<string>(@"
                                            SELECT rolname FROM (VALUES 
                                                ('role_admin'), 
                                                ('role_manager'), 
                                                ('role_doctor'), 
                                                ('role_analyst')
                                            ) AS t(rolname)
                                            WHERE pg_has_role(rolname, 'MEMBER')");

                if (role == null) return (false, null);

                return (true, role);
            }
            catch
            {
                return (false, null);
            }
        }


        [HttpPost("register")]
        public async Task<IActionResult> CreateUser([FromBody] RegistrationUserDTO userInfo)
        {
            DatabaseUserDTO user = userInfo.DatabaseUserDTO;
            EmployeeTableDTO employee = userInfo.EmployeeTableDTO;

            if (_context.Employees.Any(e => e.DbUsername == user.DbUsername)) return BadRequest("Логин занят");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var clinic = await _context.Clinics
                                   .FirstOrDefaultAsync(c => c.Location == userInfo.ClinicLocation);

                //create user in database
                await _context.Database.ExecuteSqlRawAsync(
                    $"CREATE USER \"{user.DbUsername}\" WITH PASSWORD '{user.DbPassword}'"
                );

                await _context.Database.ExecuteSqlRawAsync($"GRANT \"{user.Role}\" TO \"{user.DbUsername}\"");

                //add new employee into Employee table
                Employee newEmployee = new Employee
                {
                    FirstName = employee.FirstName,
                    SecondName = employee.SecondName,
                    PhoneNumber = employee.PhoneNumber,
                    Specialization = employee.Specialization,
                    Info = employee.Info,
                    Age = employee.Age,
                    Salary = employee.Salary,
                    Experience = employee.Experience,
                    DbUsername = employee.DbUsername
                };

                await _context.Employees.AddAsync(newEmployee);

                await _context.SaveChangesAsync();

                
                ClinicEmployee a = new ClinicEmployee
                {
                    EmployeeId = newEmployee.Id,  
                    ClinicId = clinic.Id
                };

                await _context.ClinicEmployees.AddAsync(a);
                

                int[] categories = GetCategoryIdsForSpecialization(newEmployee.Specialization);

                for (int i = 0; i < categories.Length; i++)
                {
                    DoctorCategorySkill b = new DoctorCategorySkill
                    {
                        EmployeeId = newEmployee.Id,
                        CategoryId = categories[i]
                    };

                    await _context.DoctorCategorySkills.AddAsync(b);
                }

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(ex.Message);
            }           

            
            return Ok("Успех");
        }
        private int[] GetCategoryIdsForSpecialization(string specialization)
        {
            return specialization switch
            {
                "Стоматолог-терапевт" => new[] { 1, 2, 8 },      // Диагностика, Терапия, Отбеливание
                "Стоматолог-хирург" => new[] { 4, 5 },         // Хирургия, Имплантация
                "Стоматолог-ортопед" => new[] { 6 },            // Протезирование
                "Ортодонт" => new[] { 7 },            // Ортодонтия
                "Пародонтолог" => new[] { 3, 4 },         // Гигиена, Хирургия (лечение дёсен)
                "Эндодонтист" => new[] { 2 },            // Терапия (лечение каналов)
                "Детский стоматолог" => new[] { 1, 2, 3, 4, 7, 8 }, // всё, кроме имплантации и протезирования
                "Стоматолог-гигиенист" => new[] { 3, 8 },         // Гигиена, Отбеливание
                _ => Array.Empty<int>()
            };
        }
    }
}
