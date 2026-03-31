using DoctorMomFrontend.Utils;
using Microsoft.VisualBasic;
using System.Linq;
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
        private DateTime _today = DateTime.Now;
        private List<CheckDTO> _checksData;
        private List<ClinicTableDTO> _clinicsData;
        private List<AppointmentDTO> _appointmentsData;
        private List<ServiceDTO> _servicesData;
        private List<ClientDTO> _clientsData;
        private List<EmployeeTableDTO> _doctorsData;
        private List<EmployeeTableDTO> _employeesData;
        public AnalyticsDashboardPage()
        {
            InitializeComponent();
            LogoutButton.Click += (s, e) =>
            {
                NavigationService.Navigate(new AuthorizePage());
                EmployeeSession.Clear();
            };
            Loaded += async (s, e) => await LoadAnalyticsDataAsync();
        }

        private async Task LoadAnalyticsDataAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                await LoadAllData(client);
                await LoadCommonStatistic(client);
                await LoadFiltresData();
                await LoadGeneralSummary();
                await LoadFinancialData();
            }
        }

        private async Task LoadFinancialData()
        {
            var clinicData = new List<ClinicFinancialMV>();
            foreach (var clinic in _clinicsData)
            {
                var clinicAppointments = _appointmentsData
                        .Where(a => a.ClinicId == clinic.Id 
                            && a.Status == AppointmentStatus.Completed);

                if (clinicAppointments.Count() == 0) continue;

                var clinicInfo = new ClinicFinancialMV
                {
                    ClinicAddress = clinic.Location,
                    Revenue = clinicAppointments.Sum(a => a.TotalPrice),
                    BonusesDiscount = clinicAppointments.Sum(a => a.Discount),
                    EmployeeCount = clinic.EmployeesCount,
                    AvarageCheck = (int)clinicAppointments.Average(c => c.TotalPrice),
                    AvarageSalary = (int)_employeesData.Average(e => e.Salary),
                    ChecksCount = clinicAppointments.Count()
                };

                clinicData.Add(clinicInfo);
            }
            FinanceDataGrid.ItemsSource = clinicData;

        }

        private async Task LoadAllData(HttpClient client)
        {
            try
            {
                var checksResponse = await client.GetAsync(_apiUrl + "appointments/checks");
                var servicesResponse = await client.GetAsync(_apiUrl + "clinics/services");
                var clientsResponse = await client.GetAsync(_apiUrl + "clients");
                var appointmentsResponse = await client.GetAsync(_apiUrl + "appointments");
                var doctorsResponse = await client.GetAsync(_apiUrl + "employees/doctors");
                var clinicsResponse = await client.GetAsync(_apiUrl + "clinics");
                var employeesResponse = await client.GetAsync(_apiUrl + "employees");

                if (checksResponse.IsSuccessStatusCode && doctorsResponse.IsSuccessStatusCode &&
                    appointmentsResponse.IsSuccessStatusCode && employeesResponse.IsSuccessStatusCode &&
                    servicesResponse.IsSuccessStatusCode &&
                    clientsResponse.IsSuccessStatusCode &&
                    clinicsResponse.IsSuccessStatusCode)
                {
                    _employeesData = await employeesResponse.Content.ReadFromJsonAsync<List<EmployeeTableDTO>>() ?? new();
                    _doctorsData = await doctorsResponse.Content.ReadFromJsonAsync<List<EmployeeTableDTO>>() ?? new();
                    _checksData = await checksResponse.Content.ReadFromJsonAsync<List<CheckDTO>>() ?? new();
                    _servicesData = await servicesResponse.Content.ReadFromJsonAsync<List<ServiceDTO>>() ?? new();
                    _clientsData = await clientsResponse.Content.ReadFromJsonAsync<List<ClientDTO>>() ?? new();
                    _clinicsData = await clinicsResponse.Content.ReadFromJsonAsync<List<ClinicTableDTO>>() ?? new();
                    _appointmentsData = await appointmentsResponse.Content.ReadFromJsonAsync<List<AppointmentDTO>>() ?? new();
                }
                else
                {
                    MessageBox.Show($"Ошибка при загрузке данных", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async Task LoadFiltresData()
        {
            var clinicsList = _clinicsData.Select(c => c.Location).ToList();
            clinicsList.Insert(0, "Все клиники"); 
            ClinicFilter.ItemsSource = clinicsList;
            ClinicFilter.SelectedIndex = 0; 

            var doctorsList = _doctorsData.Select(d => d.Specialization).Distinct().ToList();
            doctorsList.Insert(0, "Все доктора");
            DoctorFilter.ItemsSource = doctorsList;
            DoctorFilter.SelectedIndex = 0;

            var servicesList = _servicesData.Select(s => s.Name).ToList();
            servicesList.Insert(0, "Все услуги");
            ServiceFilter.ItemsSource = servicesList;
            ServiceFilter.SelectedIndex = 0;

            FromDateFilter.SelectedDate = _today;
            ToDateFilter.SelectedDate = _today.AddMonths(1);
        }        
        private async Task LoadCommonStatistic(HttpClient client)
        {
            try
            {
                TotalRevenueText.Text = _checksData.Sum(c => c.TotalPrice).ToString("C");
                TotalAppointmentsText.Text = _appointmentsData.Count.ToString();
                CompletedAppointmentsText.Text = _appointmentsData.Count(a => a.Status == AppointmentStatus.Completed).ToString();
                CancelledAppointmentsText.Text = _appointmentsData.Count(a => a.Status == AppointmentStatus.Cancelled).ToString();
                AverageCheckText.Text = _checksData.Count > 0 ? (_checksData.Average(c => c.TotalPrice)).ToString("C") : "N/A";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке статистики: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async Task LoadGeneralSummary()
        {
            try
            {
                int mostFrequentServiceId = _checksData
                    .Where(c => c.Date.Month == _today.Month)
                    .GroupBy(s => s.ServiceId)
                    .OrderByDescending(s => s.Count())
                    .Select(s => s.Key)
                    .FirstOrDefault();
                
                int mostProfitableServiceId = _checksData
                    .Where(c => c.Date.Month == _today.Month)
                    .GroupBy(c => c.ServiceId)
                    .OrderByDescending(g => g.Sum(c => c.TotalPrice))
                    .Select(g => g.Key)
                    .FirstOrDefault();

                int topDoctorId = _appointmentsData
                    .Where(c => c.Date.Month == _today.Month)
                    .GroupBy(a => a.EmployeeId)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .FirstOrDefault();

                int mostBisiestClinicId = _appointmentsData
                    .Where(c => c.Date.Month == _today.Month)
                    .GroupBy(a => a.ClinicId)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .FirstOrDefault();

                ServiceDTO mostFrequentService = _servicesData
                    .FirstOrDefault(s => s.Id == mostFrequentServiceId) 
                    ?? new ServiceDTO { Name = "N/A" };

                ServiceDTO mostProfitableService = _servicesData
                    .FirstOrDefault(s => s.Id == mostProfitableServiceId) 
                    ?? new ServiceDTO { Name = "N/A" };

                EmployeeTableDTO topDoctor = _doctorsData
                    .FirstOrDefault(d => d.Id == topDoctorId) 
                    ?? new EmployeeTableDTO 
                    {
                        FirstName = "N/A",
                        SecondName = "",
                    };

                string clinicAddress = _clinicsData
                    .FirstOrDefault(c => c.Id == mostBisiestClinicId).Location ?? "N/A";

                decimal serviceRevenue = _checksData
                    .Where(c => c.ServiceId == mostProfitableServiceId)
                    .Sum(c => c.TotalPrice);
                int serviceCount = _checksData
                    .GroupBy(s => s.ServiceId)
                    .OrderByDescending(s => s.Count())
                    .Select(s => s.Count())
                    .FirstOrDefault();
                int doctorAppointments = _appointmentsData
                    .Count(a => a.EmployeeId == topDoctor.Id);

                MostFrequentServiceNameText.Text = mostFrequentService.Name;
                MostFrequentServiceCountText.Text = serviceCount.ToString();
                MostProfitableServiceNameText.Text = mostProfitableService.Name;
                MostProfitableServiceRevenueText.Text = serviceRevenue.ToString("C");
                TopDoctorNameText.Text = topDoctor.FullName + $" ({topDoctor.Specialization})";
                TopDoctorAppointmentsText.Text = doctorAppointments.ToString();
                ActiveClinicsCountText.Text = clinicAddress;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке общего отчета: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
