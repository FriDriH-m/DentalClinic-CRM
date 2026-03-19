using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.X509Certificates;

namespace DoctorMomFrontend.Utils
{
    public enum AppointmentStatus { Pending = 0, Completed = 1, Cancelled = 2 }
    public static class EmployeeSession
    {
        public static int EmployeeId { get; set; }
        public static string Role { get; set; }
        public static int[] ClinicsIds { get; set; }

        public static void Clear()
        {
            EmployeeId = 0;
            Role = null;
        }
    }
    public record SessionInfo(int EmployeeId, string Role, int[] ClinicsId);
    public record LoginUserDTO(string Login, string Password);
    public class ClinicTableDTO 
    {
        public int Id { get; set; }
        public string Location { get; set; }
        public string PostalCode { get; set; }
        public string PhoneNumber { get; set; }
        public int EmployeesCount { get; set; }
    }
    public class AppointmentDTO
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public AppointmentStatus Status { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Discount { get; set; }
        public bool IsClosed { get; set; }
        public int ClientId { get; set; }
        public int ClinicId { get; set; }
        public int EmployeeId { get; set; }
    }
    public class MaterialDTO 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsCertifiedMaterial { get; set; }
        public decimal Price { get; set; }
        public decimal PurchasePrice { get; set; }
        public int Count { get; set; }
        public int ClinicId { get; set; }
        public string ClinicAddress { get; set; }
    }
    public record RegisterService(ServiceDTO service, int[] materialsId);
    public class RegistrationUserDTO
    {
        public EmployeeTableDTO EmployeeTableDTO { get; set; }
        public DatabaseUserDTO DatabaseUserDTO { get; set; }
        public string ClinicLocation { get; set; }
    }
    public class ServiceDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DurationMinutes { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public decimal BasePrice { get; set; }
        public int ClinicId { get; set; }
        public string ClinicAddress { get; set; }
    }
    public class EmployeeTableDTO
    {
        public int Id { get; set; }
        public string FullName => $"{SecondName} {FirstName}".Trim();
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string PhoneNumber { get; set; }
        public string Specialization { get; set; }
        public string Info { get; set; }
        public bool IsCertified { get; set; }
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
