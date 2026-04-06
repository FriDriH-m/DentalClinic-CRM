using DoctorMomFrontend.Extensions;
using DoctorMomFrontend.Utils;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;

namespace DoctorMomFrontend
{
    /// <summary>
    /// Логика взаимодействия для ManagerAppointmentsPage.xaml
    /// </summary>
    public partial class ManagerAppointmentsPage : Page
    {
        private readonly string ApiUrl = "https://localhost:7141/api/";
        private List<ServiceDTO> _allAvailableServices = new();
        private List<ClientDTO> _allClients = new();
        private Dictionary<int, int> _allBonuses = new();        
        private EmployeeTableDTO _selectedDoctor;
        public ObservableCollection<ServiceDTO> Services { get; set; } = new();
        public ManagerAppointmentsPage()
        {
            InitializeComponent();
            InitializeTimeComboBoxes();
            DataContext = this;
            Loaded += async (s, e) => await GetDataFromDB();

            LogoutButton.Click += (s, e) =>
            {
                NavigationService.Navigate(new AuthorizePage());
                EmployeeSession.Clear();
            };

            ServiceComboBox.SelectionChanged += UpdateServiceInfo;
            DoctorsListBox.SelectionChanged += UpdateDoctorSelect;
            PatientsDataGrid.SelectionChanged += UpdateClientInfo;

            PatientSearchTextBox.TextChanged += UpdateClientsList;

            SelectDoctorButton.Click += SaveDoctorChoose;
            SaveButton.Click += async (s, e) => await CreateAppointment();
            BackButton.Click += (s, e) => NavigationService.GoBack();
            AppointmentsPageButton.Click += (s, e) => NavigationService.Navigate(new ManagerAppointmentsPage());
            ClientsPageButton.Click += (s, e) => NavigationService.Navigate(new ManagerClientsPage());
            AddPatientButton.Click += (s, e) => NavigationService.Navigate(new RegistrationClientPage());
            FindDoctorsButton.Click += async (s, e) => await FindFreeDoctors();
        }
        private async Task CreateAppointment()
        {
            if (_selectedDoctor == null)
            {
                MessageBox.Show("Выберите доктора");
                return;
            }            
            var serviceId = (int)ServiceComboBox.SelectedValue;
            var service = Services.First(s => s.Id == serviceId);
            var clientDTO = PatientsDataGrid.SelectedItem as ClientDTO;

            DateTime date = (DateTime)DatePick.SelectedDate;
            date = date.AddHours(int.Parse(HourComboBox.SelectedValue.ToString()));
            date = date.AddMinutes(int.Parse(MinuteComboBox.SelectedValue.ToString()));
            DateTime endTime = date.AddMinutes(service.DurationMinutes);

            decimal totalPrice = service.BasePrice;            
            decimal discount = 0;


            if (UseBonusesCheckBox.IsChecked ?? false)
            {
                if (clientDTO.BonuseAmount != 0)
                {
                    if (clientDTO.BonuseAmount >= totalPrice) discount = (decimal)totalPrice;
                    else discount = (decimal)_allBonuses[clientDTO.Id];
                }
            }
            using (HttpClient client = new HttpClient())
            {
                var checkAvailability = await client.GetAsync(ApiUrl + "clinics/services/" + serviceId + "/materials/availability");
                if (!checkAvailability.IsSuccessStatusCode)
                {
                    MessageBox.Show("Недостаточно материалов для услуги");
                    return;
                }
                try
                {
                    AppointmentDTO newAppointment = new AppointmentDTO
                    {
                        Date = date,
                        EndTime = endTime,
                        Discount = discount,
                        TotalPrice = totalPrice - discount,
                        Status = AppointmentStatus.Pending,
                        IsClosed = false,
                        ServiceId = serviceId,
                        ClientId = clientDTO.Id,
                        ClinicId = EmployeeSession.ClinicsIds[0],
                        EmployeeId = _selectedDoctor.Id
                    };
                    newAppointment.Date = DateTime.SpecifyKind(newAppointment.Date, DateTimeKind.Utc);
                    newAppointment.EndTime = DateTime.SpecifyKind(newAppointment.EndTime, DateTimeKind.Utc);

                    var response = await client.PostAsJsonAsync(ApiUrl + "appointments", newAppointment);
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Запись создана");
                        NavigationService.GoBack();
                    }
                    else
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();

                        MessageBox.Show($"Ошибка: {errorMessage}", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }                
            }
        }
        private void UpdateClientsList(object sender, TextChangedEventArgs e)
        {
            PatientsDataGrid.ItemsSource = _allClients.Where(c => c.FullName.Contains(PatientSearchTextBox.Text));
        }
        private void UpdateClientInfo(object sender, SelectionChangedEventArgs e)
        {
            ClientInfoPanel.Visibility = Visibility.Visible;
            var client = PatientsDataGrid.SelectedValue as ClientDTO;

            ClientNameText.Text = client?.FullName ?? "";
            ClientPhoneText.Text = client?.PhoneNumber ?? "";
            ClientEmailText.Text = client?.Email ?? "";
            ClientNotesText.Text = client?.Info ?? "";
        }
        private async Task LoadClients(HttpClient client)
        {
            try
            {
                var clientsResponse = await client.GetAsync(ApiUrl + "clients");
                var bonusesResponse = await client.GetAsync(ApiUrl + "clients/bonuses");
                if (clientsResponse.IsSuccessStatusCode && bonusesResponse.IsSuccessStatusCode)
                {
                    _allClients = await clientsResponse.Content.ReadFromJsonAsync<List<ClientDTO>>();

                    _allBonuses = await bonusesResponse.Content.ReadFromJsonAsync<Dictionary<int, int>>();

                    foreach (var currentClient in _allClients)
                    {
                        if (!_allBonuses.ContainsKey(currentClient.Id)) currentClient.BonuseAmount = 0;
                        else currentClient.BonuseAmount = _allBonuses[currentClient.Id];
                    }

                    PatientsDataGrid.ItemsSource = _allClients;
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
        private void SaveDoctorChoose(object sender, RoutedEventArgs e)
        {
            DoctorsListBox.Visibility = Visibility.Collapsed;
            SelectedDoctorPanel.Visibility = Visibility.Visible;
            SelectDoctorButton.Visibility = Visibility.Collapsed;
        }
        private void UpdateDoctorSelect(object sender, SelectionChangedEventArgs e)
        {
            if (DoctorsListBox.SelectedValue == null) return;

            SelectDoctorButton.Visibility = Visibility.Visible;

            var doctor = DoctorsListBox.SelectedValue as EmployeeTableDTO;
            _selectedDoctor = doctor;
            SelectedDoctorText.Text = doctor.FullName + " " + doctor.Specialization;
        }
        private void InitializeTimeComboBoxes()
        {
            for (int hour = 8; hour <= 22; hour++)
            {
                HourComboBox.Items.Add(hour.ToString("D2")); 
            }
            HourComboBox.SelectedIndex = 0; 

            for (int minute = 0; minute <= 50; minute += 10)
            {
                MinuteComboBox.Items.Add(minute.ToString("D2")); 
            }
            MinuteComboBox.SelectedIndex = 0; 
        }
        private async Task FindFreeDoctors()
        {
            if (HourComboBox.SelectedItem == null || MinuteComboBox.SelectedItem == null
                || DatePick.SelectedDate == null || ServiceComboBox.SelectedValue == null)
            {
                MessageBox.Show("Заполните все поля");
                return;
            }

            DoctorsListBox.Visibility = Visibility.Visible;
            SelectDoctorButton.Visibility = Visibility.Visible;

            DateTime date = (DateTime)DatePick.SelectedDate;

            // эти метода оказывается возвращают новое значение, поэтому надо присваивать
            date = date.AddHours(int.Parse(HourComboBox.SelectedValue.ToString()));
            date = date.AddMinutes(int.Parse(MinuteComboBox.SelectedValue.ToString()));

            int serviceId = (int)ServiceComboBox.SelectedValue;

            await GetDoctorsAsync(date, serviceId);
        }
        private async Task GetDoctorsAsync(DateTime dateTime, int serviceId)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.AddHeaders();

                    var dateString = dateTime.ToString("yyyy-MM-ddTHH:mm:ss");

                    var response = await client.GetAsync(ApiUrl + $"appointments/free_doctors?datetime={dateString}&serviceId={serviceId}");
                    if (response.IsSuccessStatusCode)
                    {
                        List<EmployeeTableDTO> doctors = await response.Content.ReadFromJsonAsync<List<EmployeeTableDTO>>();
                        DoctorsListBox.ItemsSource = doctors;
                    }
                    else
                    {
                        MessageBox.Show("Ошибка сервера");
                    }
                }
                catch
                {
                    MessageBox.Show("Не удалось  полоучить докторов");
                }
            }
        }
        private void UpdateServiceInfo(object sender, SelectionChangedEventArgs e)
        {
            if (DoctorsListBox.Visibility != Visibility.Collapsed)
            {
                DoctorsListBox.ItemsSource = null;
                DoctorsListBox.Visibility = Visibility.Collapsed;
            }
            _selectedDoctor = null;            
            SelectedDoctorText.Text = string.Empty;
            SelectedDoctorPanel.Visibility = Visibility.Collapsed;
            int serviceId = (int)ServiceComboBox.SelectedValue;

            var selectedService = Services.FirstOrDefault(s => s.Id == serviceId);
            if (selectedService != null)
            {
                ServiceDescriptionText.Text = selectedService.Description;
                ServicePriceText.Text = Convert.ToString(selectedService.BasePrice) + " ₽";
                ServiceDurationText.Text = Convert.ToString(selectedService.DurationMinutes) + " минут";
            }
        }
        private async Task GetDataFromDB()
        {
            using (HttpClient client = new HttpClient())
            {
                client.AddHeaders();

                await LoadServices(client);
                await LoadClients(client);
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

            foreach (var item in _allAvailableServices) Services.Add(item);
        }
    }
}
