using System.ComponentModel.DataAnnotations.Schema;
using Web_API.Models;

namespace Web_API.Utils
{
    public record SessionInfo(int EmployeeId, string Role, int[] ClinicsId);
    public record LoginUserDTO(string Login, string Password);
    public record RegisterService(ServiceDTO service, int[] materialsId);
    public class RegistrationUserDTO
    {
        public EmployeeTableDTO EmployeeTableDTO { get; set; }
        public DatabaseUserDTO DatabaseUserDTO { get; set; }
        public string ClinicLocation { get; set; }
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
        public string ClinicAddress {  get; set; }
    }
    public class Appointment
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public AppointmentStatus Status { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Discount { get; set; }
        public int ClientId { get; set; }
        public int ClinicId { get; set; }
        public int EmployeeId { get; set; }
    }
    public class ClinicTableDTO
    {
        public int Id { get; set; }
        public string Location { get; set; }
        public string PostalCode { get; set; }
        public string PhoneNumber { get; set; }
        public int EmployeesCount { get; set; }
    }
    public class EmployeeTableDTO
    {
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
