using DoctorMomFrontend.Utils;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;

namespace DoctorMomFrontend
{
    /// <summary>
    /// Логика взаимодействия для AnalystMainPage.xaml
    /// </summary>
    public partial class AnalyticsDashboardPage : Page
    {
        private readonly string _apiUrl = "https://localhost:7141/api/";
        private List<CheckDTO> _checksData;
        private List<AppointmentDTO> _appointmentsData;
        private List<ServiceDTO> _servicesData;
        private List<ClientDTO> _clientsData;
        public AnalyticsDashboardPage()
        {
            InitializeComponent();
            Loaded += async (s, e) => await LoadAnalyticsDataAsync();
        }

        private async Task LoadAnalyticsDataAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                await LoadNeedData(client);
                await LoadStatistic(client);
            }
        }

        private async Task LoadNeedData(HttpClient client)
        {
            try
            {
                var checksResponse = await client.GetAsync(_apiUrl + "appointments/checks");
                var servicesResponse = await client.GetAsync(_apiUrl + "clinics/services");
                var clientsResponse = await client.GetAsync(_apiUrl + "clients");
                var appointmentsResponse = await client.GetAsync(_apiUrl + "appointments"); 
                
                if (checksResponse.IsSuccessStatusCode &&
                    appointmentsResponse.IsSuccessStatusCode &&
                    servicesResponse.IsSuccessStatusCode &&
                    clientsResponse.IsSuccessStatusCode)
                {
                    _checksData = await checksResponse.Content.ReadFromJsonAsync<List<CheckDTO>>() ?? new();
                    _servicesData = await servicesResponse.Content.ReadFromJsonAsync<List<ServiceDTO>>() ?? new();
                    _clientsData = await clientsResponse.Content.ReadFromJsonAsync<List<ClientDTO>>() ?? new();
                    _appointmentsData = await appointmentsResponse.Content.ReadFromJsonAsync<List<AppointmentDTO>>() ?? new();
                }
                else
                {
                    var error = await checksResponse.Content.ReadAsStringAsync();
                    MessageBox.Show($"Ошибка при загрузке данных: {error}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadStatistic(HttpClient client)
        {
            TotalRevenueText.Text = _checksData.Sum(c => c.TotalPrice).ToString("C");
            TotalAppointmentsText.Text = _appointmentsData.Count.ToString();
            CompletedAppointmentsText.Text = _appointmentsData.Count(a => a.Status == AppointmentStatus.Completed).ToString();
            CancelledAppointmentsText.Text = _appointmentsData.Count(a => a.Status == AppointmentStatus.Cancelled).ToString();
            AverageCheckText.Text = _checksData.Count > 0 ? (_checksData.Average(c => c.TotalPrice)).ToString("C") : "N/A";
        }
    }
}
