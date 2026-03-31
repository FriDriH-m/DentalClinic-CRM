using System.ComponentModel.DataAnnotations.Schema;
using Web_API.Models;

namespace Web_API.Utils
{
    public record SessionInfo(int EmployeeId, string Role, int[] ClinicsId);
    public record LoginUserDTO(string Login, string Password);
    public record RegisterService(ServiceDTO service, Dictionary<int, int> materialsId);
    public record ServiceMaterialDTO(ServiceDTO service, MaterialDTO material);
    public class RegistrationUserDTO
    {
        public EmployeeTableDTO EmployeeTableDTO { get; set; }
        public DatabaseUserDTO DatabaseUserDTO { get; set; }
        public string ClinicLocation { get; set; }
    }
    public record AppointmentMaterialsChange(AppointmentDTO appointmentDTO, decimal priceChange);
    public class CheckDTO
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Discount { get; set; }
        public string ClientName { get; set; }
        public string EmployeeName { get; set; }
        public string ServiceName { get; set; }
        public AppointmentDTO Appointment { get; set; }
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
        public string ClinicAddress { get; set; }
        public int ClinicId { get; set; }
    }
    public class ClientDTO
    {
        public string FullName => $"{SecondName} {FirstName}".Trim();
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Info { get; set; }
        public ClientStatus Status { get; set; }
        public int MoneySpent { get; set; }
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
    public class ClinicTableDTO
    {
        public int Id { get; set; }
        public string Location { get; set; }
        public string PostalCode { get; set; }
        public string PhoneNumber { get; set; }
        public int EmployeesCount { get; set; }
    }
    public class AppointmentMaterialDTO
    {
        public int AppointmentId { get; set; }
        public int MaterialId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        
    }
    public class BonuseDTO
    {
        public int Amount { get; set; }
        public int ClientId { get; set; }
    }
    public class AppointmentDTO
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime EndTime { get; set; }
        public AppointmentStatus Status { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Discount { get; set; }
        public bool IsClosed { get; set; }
        public int ClientId { get; set; }
        public int ClinicId { get; set; }
        public int EmployeeId { get; set; }
        public int ServiceId { get; set; }
        
        public Dictionary<int, int>? MaterialsId { get; set; }
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
