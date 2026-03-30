using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_API.Models
{
    public enum ClientStatus { Regular = 0, Loyal = 1, Premium = 2 }
    public enum AppointmentStatus { Pending = 0, Completed = 1, Cancelled = 2 }
    public class ClinicEmployee
    {
        public int Id { get; set; } 

        public int EmployeeId { get; set; }
        public int ClinicId { get; set; }

        [ForeignKey("EmployeeId")] public Employee Employee { get; set; }
        [ForeignKey("ClinicId")] public Clinic Clinic { get; set; }
    }
    public class AppointmentService
    {
        public int AppointmentId { get; set; }
        public int ServiceId { get; set; }
        [ForeignKey("AppointmentId")] public Appointment Appointment { get; set; }
        [ForeignKey("ServiceId")] public Service Service { get; set; }

        public decimal Price { get; set; } //чтобы при подорожании материала не менять стоимость уже созданных записей
    }
    public class AppointmentMaterial
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public int MaterialId { get; set; }
        [ForeignKey("AppointmentId")] public Appointment Appointment { get; set; }
        [ForeignKey("MaterialId")] public Material Material { get; set; }

        public int Quantity { get; set; }
        public decimal Price { get; set; } //чтобы при подорожании материала не менять стоимость уже созданных записей
    }
    public class Clinic
    {
        public int Id { get; set; }
        public string Location { get; set; }
        public string PostalCode { get; set; }
        public string PhoneNumber { get; set; }
        public int EmployeesCount { get; set; }
        public List<ClinicEmployee> ClinicEmployees { get; set; }
    }
    public class DoctorCategorySkill
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int CategoryId{ get; set; }
        [ForeignKey("EmployeeId")] public Employee Employee { get; set; }
    }
    public class Employee
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string PhoneNumber { get; set; }
        public string Specialization { get; set; }
        public string Info { get; set; }
        public bool? IsCertified { get; set; }
        public int Age { get; set; }
        public int Salary { get; set; }
        public int Experience { get; set; }
        public string DbUsername { get; set; }
        public List<Appointment> Appointments { get; set; }
        public List<ClinicEmployee> ClinicEmployees { get; set; }
        public List<DoctorCategorySkill> DoctorCategorySkills { get; set; }
        public List<DoctorMaterialAccess> MaterialsAccess { get; set; }
    }
    public class Client
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Info { get; set; }
        public ClientStatus Status { get; set; }
        public int MoneySpent { get; set; }
        public List<Appointment> Appointments { get; set; }
        public List<Bonuse> Bonuses { get; set; }
    }   
    public class Bonuse
    {
        public int Id { get; set; }
        public DateTime AddedAt { get; set; }
        public DateTime ExpiredAt { get; set; }
        public int Amount { get; set; }
        public int ClientId { get; set; }
        [ForeignKey("ClientId")] public Client Client { get; set; }
    }
    public class Material 
    {
        public int Id { get; set; }
        public string Name { get; set; }    
        public string Description { get; set; }
        public bool IsCertifiedMaterial { get; set; }
        public decimal Price { get; set; }
        public decimal PurchasePrice { get; set; }
        public int Count { get; set; }
        public int ClinicId { get; set; }
        [ForeignKey("ClinicId")] public Clinic Clinic { get; set; }

        public List<AppointmentMaterial> AppointmentMaterials { get; set; }
        public List<ServiceMaterials> Services { get; set; }
        public List<DoctorMaterialAccess> DoctorsAccess { get; set; }
    }
    public class Report 
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Note { get; set; }
        public string Author { get; set; }
        public string Path { get; set; }
        public DateTime Created { get; set; }
    }
    public class Appointment
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime EndTime { get; set; }
        public AppointmentStatus Status { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Discount { get; set; }
        public bool IsClosed {  get; set; }
        public int ClientId { get; set; }
        public int ClinicId { get; set; }        
        public int EmployeeId { get; set; }
        [ForeignKey("ClientId")] public Client Client { get; set; }
        [ForeignKey("ClinicId")] public Clinic Clinic { get; set; }
        [ForeignKey("EmployeeId")] public Employee Employee { get; set; }
        public List<AppointmentService> AppointmentService { get; set; }
        public List<AppointmentMaterial> AppointmentMaterials { get; set; }
    }
    public class Check
    {
        public int Id { get; set; }       
        public DateTime Date { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Discount { get; set; }
        public int AppointmentId { get; set; }
        public int ClientId { get; set; }
        [ForeignKey("AppointmentId")] public Appointment Appointment { get; set; }
        [ForeignKey("ClientId")] public Client Client { get; set; }
    }
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DurationMinutes { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int ClinicId { get; set; }
        public decimal BasePrice { get; set; }
        [ForeignKey("ClinicId")] public Clinic Clinic { get; set; }
        public List<AppointmentService> AppointmentService { get; set; }
        public List<ServiceMaterials> Materials { get; set; }
    }    
    public class ServiceMaterials 
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public int MaterialId { get; set; }
        public int Count { get; set; }

        [ForeignKey("ServiceId")] public Service Service { get; set; }
        [ForeignKey("MaterialId")] public Material Material { get; set; }
    }
    public class DoctorMaterialAccess
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int MaterialId { get; set; }

        [ForeignKey("EmployeeId")] public Employee Employee { get; set; }
        [ForeignKey("MaterialId")] public Material Material { get; set; }
    }
}
