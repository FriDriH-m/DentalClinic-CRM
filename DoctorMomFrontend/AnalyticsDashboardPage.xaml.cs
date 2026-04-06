using DoctorMomFrontend.Utils;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;

namespace DoctorMomFrontend
{
    public enum AnalyticsTab
    {
        GeneralSummary,
        Financial,
        Services,
        Doctors,
        Clients
    }
    /// <summary>
    /// Логика взаимодействия для AnalystMainPage.xaml
    /// </summary>
    public partial class AnalyticsDashboardPage : Page
    {
        private readonly string _apiUrl = "https://localhost:7141/api/";
        public TabItem SelectedTab;
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
            DataContext = this;
            
            //AnalyticsTabs.SelectionChanged +=
        }

        private async void AnalyticsTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AnalyticsTabs.SelectedItem is TabItem selectedTab)
            {
                SelectedTab = selectedTab;
                if (selectedTab.Header.ToString() == "Общая сводка")
                {
                }
                else if (selectedTab.Header.ToString() == "💰 Финансы")
                {
                }
                else if (selectedTab.Header.ToString() == "🏥 Услуги")
                {
                }
                else if (selectedTab.Header.ToString() == "👨‍⚕️ Врачи")
                {
                }
                else if (selectedTab.Header.ToString() == "👥 Клиенты")
                {
                }
            }
        }

        private async Task LoadAnalyticsDataAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                await LoadAllData(client);
                await LoadCommonStatistic(client);
                await LoadFiltresData();
                await LoadGeneralSummary();
                await LoadFinancialStatisticData();
                await LoadServiceStatisticData();
                await LoadDoctorsStatisticData();
                await LoadClientsStatisticData();
            }
        }

        private async Task LoadClientsStatisticData()
        {
            try 
            { 
                var clientsData = new List<ClientModelView>();
                foreach (var client in _clientsData)
                {
                    var clientAppointments = _appointmentsData
                            .Where(a => a.ClientId == client.Id);
                    var clientData = new ClientModelView
                    {
                        FullName = client.FullName,
                        BonuseSpent = (int)clientAppointments
                            .Where(a => a.Status == AppointmentStatus.Completed)
                            .Sum(a => a.Discount),
                        AppointmentsCount = clientAppointments.Count(),
                        CancelledAppointments = clientAppointments
                            .Where(a => a.Status == AppointmentStatus.Cancelled)
                            .Count(),
                        CompletedAppointments = clientAppointments
                            .Where(a => a.Status == AppointmentStatus.Completed)
                            .Count(),
                        MoneySpent = (int)clientAppointments
                            .Where(a => a.Status == AppointmentStatus.Completed)
                            .Sum(a => a.TotalPrice),
                        LastVisit = clientAppointments
                            .Where(a => a.Status == AppointmentStatus.Completed)
                            .OrderByDescending(a => a.Date)
                            .Select(a => a.Date)
                            .FirstOrDefault()
                    };
                    clientsData.Add(clientData);
                }
                ClientsDataGrid.ItemsSource = clientsData;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузки статистики по клиентам: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async Task LoadDoctorsStatisticData()
        {
            var doctorsData = new List<DoctorModelView>();

            foreach (var doctor in _doctorsData)
            {
                var doctorAppointments = _appointmentsData
                        .Where(a => a.EmployeeId == doctor.Id);
                var doctorData = new DoctorModelView
                {
                    FullName = doctor.FullName,
                    Salary = doctor.Salary,
                    AppointmentCount = doctorAppointments.Count(),
                    CancelledRate = doctorAppointments
                        .Where(a => a.Status == AppointmentStatus.Cancelled)
                        .Count(),
                    CompletedRate = doctorAppointments
                        .Where(a => a.Status == AppointmentStatus.Completed)
                        .Count(),
                    Revenue = (int)doctorAppointments
                        .Where(a => a.Status == AppointmentStatus.Completed)
                        .Sum(a => a.TotalPrice),
                    ClinicLocation = _clinicsData
                        .FirstOrDefault(c => c.Id == doctor.ClinicId)?
                        .Location ?? ""
                };
                doctorsData.Add(doctorData);
            }
            DoctorsDataGrid.ItemsSource = doctorsData;
        }
        private async Task LoadFinancialStatisticData()
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
                    Salary = _employeesData.Sum(e => e.Salary),
                    ChecksCount = clinicAppointments.Count()
                };

                clinicData.Add(clinicInfo);
            }
            FinanceDataGrid.ItemsSource = clinicData;
        }
        private async Task LoadServiceStatisticData()
        {
            var services = new List<ServiceModelView>();
            var serviceAppointments = _appointmentsData
                        .Where(a => a.Status == AppointmentStatus.Completed);
            foreach (var service in _servicesData)
            {
                var currentServiceAppointments = serviceAppointments
                        .Where(a => a.ServiceId == service.Id);
                int topClinicId = currentServiceAppointments
                        .GroupBy(a => a.ClinicId)
                        .OrderByDescending(a => a.Count())
                        .Select(s => s.Key)
                        .FirstOrDefault();
                var serviceData = new ServiceModelView
                {
                    Name = service.Name,
                    CategoryName = service.CategoryName,
                    Count = currentServiceAppointments.Count(),
                    Revenue = (int)currentServiceAppointments.Sum(a => a.TotalPrice),
                    TopClinic = _clinicsData.FirstOrDefault(c => c.Id == topClinicId)?.Location ?? ""
                };
                services.Add(serviceData);
            }
            ServicesDataGrid.ItemsSource = services;
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
                int completedAppointmentsCount = _appointmentsData.Count(a => a.Status == AppointmentStatus.Completed);
                int cancelledAppointmentsCount = _appointmentsData.Count(a => a.Status == AppointmentStatus.Cancelled);
                TotalRevenueText.Text = _checksData.Sum(c => c.TotalPrice).ToString("C");
                TotalAppointmentsText.Text = _appointmentsData.Count.ToString();
                CompletedAppointmentsText.Text = completedAppointmentsCount.ToString();
                CancelledAppointmentsText.Text = cancelledAppointmentsCount.ToString();
                AverageCheckText.Text = _checksData.Count > 0 ? (_checksData.Average(c => c.TotalPrice)).ToString("C") : "N/A";

                float completePercent = (float)completedAppointmentsCount / (completedAppointmentsCount + cancelledAppointmentsCount);
                float cancelPercent = (float)cancelledAppointmentsCount / (completedAppointmentsCount + cancelledAppointmentsCount);
                CompletionRateText.Text = (completePercent * 100).ToString() + "%";
                CancellationRateText.Text = (cancelPercent * 100).ToString() + "%";
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
                    .FirstOrDefault(c => c.Id == mostBisiestClinicId)?.Location ?? "N/A";

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
