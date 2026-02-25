using System.ComponentModel.DataAnnotations.Schema;

namespace Web_API.Models
{
    public enum ClientStatus { Regular = 0, Loyal = 1, Premium = 2 }
    public enum AppointmentStatus { Pending = 0, Completed = 1, Cancelled = 2 }
    public class Clinic
    {
        public int Id { get; set; }
        public string Location { get; set; }
        public string PostalCode { get; set; }
        public string PhoneNumber { get; set; }
        public int EmployeesCount { get; set; }
        public List<Employee> Employees { get; set; }
    }
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Employee> Employees { get; set; }   
    }
    public class Employee
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string PhoneNumber { get; set; }
        public string Position { get; set; }
        public string Info { get; set; }
        public string Login { get; set; }
        public string PasswordHash { get; set; }
        public int Age { get; set; }
        public int Salary { get; set; }
        public int Experience { get; set; }
        public int RoleId { get; set; }
        public List<Appointment> Appointments { get; set; }
        public List<Clinic> Clinics { get; set; }

        [ForeignKey("RoleId")] public Role Role { get; set; }
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
        public int Bonuses { get; set; }
        public List<Appointment> Appointments { get; set; }
    }
    public class Material 
    {
        public int Id { get; set; }
        public string Name { get; set; }    
        public string Description { get; set; }
        public int Price { get; set; }
        public int Count { get; set; }
        public int ClinicId { get; set; }
        [ForeignKey("ClinicId")] public Clinic Clinic { get; set; }

        public List<Appointment> Appointments { get; set; } 
    }

    public class Appointment
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public AppointmentStatus Status { get; set; }
        public int TotalPrice { get; set; }
        public int ClientId { get; set; }
        public int ClinicId { get; set; }        
        public int EmployeeId { get; set; }
        [ForeignKey("ClientId")] public Client Client { get; set; }
        [ForeignKey("ClinicId")] public Clinic Clinic { get; set; }
        [ForeignKey("EmployeeId")] public Employee Employee { get; set; }
        public List<Material> Materials { get; set; }
        public List<Service> Services { get; set; }
    }
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public List<Employee> Employees { get; set; }
        public List<Appointment> Appointments { get; set; } 
    }
}
