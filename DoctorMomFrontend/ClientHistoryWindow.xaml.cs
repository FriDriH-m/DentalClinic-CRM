using DoctorMomFrontend.Utils;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;

namespace DoctorMomFrontend
{
    /// <summary>
    /// Логика взаимодействия для ClientHistoryWindow.xaml
    /// </summary>
    public partial class ClientHistoryWindow : Window
    {
        private readonly string ApiUrl = "https://localhost:7141/api/";
        private readonly ObservableCollection<AppointmentModelView> _appointments = new();
        private List<ServiceDTO> _allAvailableServices = new();
        private List<EmployeeTableDTO> _allDoctors = new();
        private readonly ClientDTO _client;
        public ClientHistoryWindow(ClientDTO clientDTO)
        {
            InitializeComponent();
            _client = clientDTO;
            Loaded += async (s, e) => await GetDataFromDB();
            CloseButton.Click += (s, e) => Close();
        }
        private async Task GetDataFromDB()
        {
            using (HttpClient client = new HttpClient())
            {
                await LoadServices(client);
                await LoadDoctors(client);
                await LoadClientAppointments(client);                
            }
        }
        private async Task LoadServices(HttpClient client)
        {
            for (int i = 0; i < EmployeeSession.ClinicsIds.Length; i++)
            {
                var response = await client.GetAsync(ApiUrl + "clinics/services/" + EmployeeSession.ClinicsIds[i]);
                if (response.IsSuccessStatusCode)
                {
                    _allAvailableServices = await response.Content.ReadFromJsonAsync<List<ServiceDTO>>();

                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show(error);
                }
            }
        }
        private async Task LoadDoctors(HttpClient client)
        {
            try
            {
                var response = await client.GetAsync(ApiUrl + "employees/doctors");
                if (response.IsSuccessStatusCode)
                {
                    _allDoctors = await response.Content.ReadFromJsonAsync<List<EmployeeTableDTO>>();
                }
                else
                {
                    MessageBox.Show("Ошибка на стороне сервера");
                }
            }
            catch
            {
                MessageBox.Show("Не удалось получить врачей");
            }
        }
        private async Task LoadClientAppointments(HttpClient client)
        {
            try
            {
                var response = await client.GetAsync(ApiUrl + "appointments/" + _client.Id);
                if (response.IsSuccessStatusCode)
                {
                    var appointments = await response.Content.ReadFromJsonAsync<List<AppointmentDTO>>();
                    foreach (var appointment in appointments)
                    {
                        _appointments.Add(
                            new AppointmentModelView
                            {
                                Status = appointment.Status,
                                Discount = appointment.Discount,
                                Date = appointment.Date,
                                TotalPrice = appointment.TotalPrice,
                                ServiceName = _allAvailableServices
                                        .FirstOrDefault(s => s.Id == appointment.ServiceId)?.Name ?? "",
                                EmployeeName = _allDoctors
                                        .FirstOrDefault(d => d.Id == appointment.EmployeeId)?.FullName ?? ""
                            }
                        );
                    }
                    AppointmentsGrid.ItemsSource = _appointments;

                    InitAppointmentsData();
                }
            }
            catch
            {
                MessageBox.Show("Ошибка получения записей");
            }
        }

        private void InitAppointmentsData()
        {
            TotalSpentText.Text = Convert.ToString(_appointments.Sum(a => a.TotalPrice));
            TotalVisitsText.Text = Convert.ToString(_appointments.Count);
            CancelledServicesText.Text = Convert.ToString(_appointments.Where(a => a.Status == AppointmentStatus.Cancelled).Count());
            CompletedServicesText.Text = Convert.ToString(_appointments.Where(a => a.Status == AppointmentStatus.Completed).Count());
        }
    }
}
