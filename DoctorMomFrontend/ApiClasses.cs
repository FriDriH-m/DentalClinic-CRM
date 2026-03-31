using System.ComponentModel;

namespace DoctorMomFrontend.Utils
{
    public enum ClientStatus { Regular = 0, Loyal = 1, Premium = 2 }
    public enum AppointmentStatus { Pending = 0, Completed = 1, Cancelled = 2 }
    public record ServiceMaterialDTO(ServiceDTO service, MaterialDTO material);
    public record RegisterService(ServiceDTO service, Dictionary<int, int> materialsId);
    public record SessionInfo(int EmployeeId, string Role, int[] ClinicsId);
    public record LoginUserDTO(string Login, string Password);
    public record AppointmentMaterialsChange(AppointmentDTO appointmentDTO, decimal priceChange);
    public static class EmployeeSession
    {
        public static int EmployeeId { get; set; }
        public static string Role { get; set; }
        public static int[] ClinicsIds { get; set; }

        public static void Clear()
        {
            EmployeeId = 0;
            Role = null;
            ClinicsIds = new int[0];
        }
    }   
    public class ClinicTableDTO 
    {
        public int Id { get; set; }
        public string Location { get; set; }
        public string PostalCode { get; set; }
        public string PhoneNumber { get; set; }
        public int EmployeesCount { get; set; }
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
        public int BonuseAmount { get; set; }
    }
    public class AppointmentModelView : INotifyPropertyChanged
    {
        private AppointmentStatus _status;
        public AppointmentStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }
        public DateTime Date { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Discount { get; set; }
        public string ClientName { get; set; }
        public string EmployeeName { get; set; }
        public string ServiceName { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
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
        public ClientDTO Client { get; set; }
        public ServiceDTO Service { get; set; }
        public EmployeeTableDTO Employee { get; set; }
        public ClinicTableDTO Clinic { get; set; }
        public Dictionary<int, int>? MaterialsId { get; set; }
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
    public class AppointmentMaterialDTO 
    {
        public int AppointmentId { get; set; }
        public int MaterialId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
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
}
