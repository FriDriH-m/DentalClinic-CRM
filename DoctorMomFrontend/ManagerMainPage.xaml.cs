using DoctorMomFrontend.Extensions;
using DoctorMomFrontend.Utils;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace DoctorMomFrontend
{
    /// <summary>
    /// Логика взаимодействия для ManagerMainPage.xaml
    /// </summary>
    public partial class ManagerMainPage : Page
    {
        private readonly string ApiUrl = "https://localhost:7141/api/";

        private ObservableCollection<AppointmentModelView> _allAppointments = new();
        private List<ServiceDTO> _allAvailableServices = new();
        private List<ClientDTO> _allClients = new();
        private List<EmployeeTableDTO> _allDoctors = new();
        public ManagerMainPage()
        {
            InitializeComponent();
            Loaded += async (s, e) => {
                await GetDataFromDB();
                await LoadAppointments();
            };
            LogoutButton.Click += (s, e) =>
            {
                NavigationService.Navigate(new AuthorizePage());
                EmployeeSession.Clear();
            };

            NewAppointmentButton.Click += OpenAppointmentsPage;
            AppointmentsPageButton.Click += (s, e) => NavigationService.Navigate(new ManagerAppointmentsPage());
            ClientsPageButton.Click += (s, e) => NavigationService.Navigate(new ManagerClientsPage());
        }
        private void OpenAppointmentsPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ManagerAppointmentsPage());
        }
        private async Task LoadAppointments()
        {
            using (HttpClient client = new HttpClient())
            {
                client.AddHeaders();

                try
                {
                    var response = await client.GetAsync(ApiUrl + "appointments");
                    if (response.IsSuccessStatusCode)
                    {
                        var appointments = await response.Content.ReadFromJsonAsync<ObservableCollection<AppointmentDTO>>();
                        ObservableCollection<AppointmentModelView> allAppointments = new();
                        foreach (var appointment in appointments)
                        {
                            allAppointments.Add(
                                new AppointmentModelView
                                {
                                    ClientName = _allClients.FirstOrDefault(c => c.Id == appointment.ClientId).FullName,
                                    ServiceName = _allAvailableServices.FirstOrDefault(s => s.Id == appointment.ServiceId).Name,
                                    EmployeeName = _allDoctors.FirstOrDefault(d => d.Id == appointment.EmployeeId).FullName,
                                    Date = appointment.Date,
                                    Status = appointment.Status
                                }
                            );
                        }

                        _allAppointments = allAppointments;
                        AppointmentsGrid.ItemsSource = allAppointments;
                    }
                    else MessageBox.Show("Не удалось загрузить записи");
                }
                catch
                {
                    MessageBox.Show("Ошибка загрузки записей");
                    return;
                }
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
        private async Task LoadClients(HttpClient client)
        {
            try
            {
                var clientsResponse = await client.GetAsync(ApiUrl + "clients");
                if (clientsResponse.IsSuccessStatusCode)
                {
                    _allClients = await clientsResponse.Content.ReadFromJsonAsync<List<ClientDTO>>();
                }
                else
                {
                    MessageBox.Show("Не удалось получить ответ от сервера");
                }
            }
            catch
            {
                MessageBox.Show("Не удалось получить клиентов");
            }
        }
        private async Task GetDataFromDB()
        {
            using (HttpClient client = new HttpClient())
            {
                client.AddHeaders();

                await LoadServices(client);
                await LoadClients(client);
                await LoadEmployees(client);
            }
        }
        private async Task LoadEmployees(HttpClient client)
        {
            var response = await client.GetAsync(ApiUrl + "employees/doctors");

            if (response.IsSuccessStatusCode)
            {
                _allDoctors = await response.Content.ReadFromJsonAsync<List<EmployeeTableDTO>>();
            }
            else MessageBox.Show("Не удалось загрузить сотрудников");
        }
    }
}
