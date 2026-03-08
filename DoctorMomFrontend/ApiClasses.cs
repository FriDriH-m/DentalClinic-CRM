using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;

namespace DoctorMomFrontend.Utils
{
    public static class EmployeeSession
    {
        public static int EmployeeId { get; set; }
        public static string Role { get; set; }

        public static void Clear()
        {
            EmployeeId = 0;
            Role = null;
        }
    }
    public record SessionInfo(int EmployeeId, string Role);
    public record LoginUserDTO(string Login, string Password);
    public class RegistrationUserDTO
    {
        public EmployeeTableDTO EmployeeTableDTO { get; set; }
        public DatabaseUserDTO DatabaseUserDTO { get; set; }
        public string ClinicLocation { get; set; }
    }
    public class EmployeeTableDTO
    {
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string PhoneNumber { get; set; }
        public string Specialization { get; set; }
        public string Info { get; set; }
        public int Age { get; set; }
        public int Salary { get; set; }
        public int Experience { get; set; }
        public string DbUsername { get; set; }
    }
    public class DatabaseUserDTO 
    {
        public string DbUsername { get; set; }
        public string DbPassword { get; set; }
        public string Role { get; set; }
    }
}
