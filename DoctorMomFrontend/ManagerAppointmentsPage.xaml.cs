using DoctorMomFrontend.Extensions;
using DoctorMomFrontend.Utils;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
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
        public ObservableCollection<ServiceDTO> Services { get; set; } = new();
        private EmployeeTableDTO _selectedDoctor;
        public ManagerAppointmentsPage()
        {
            InitializeComponent();
            InitializeTimeComboBoxes();
            DataContext = this;
            Loaded += async (s, e) => await LoadServices();
            Loaded += async (s, e) => await LoadClients();

            ServiceComboBox.SelectionChanged += UpdateServiceInfo;
            DoctorsListBox.SelectionChanged += UpdateDoctorSelect;
            PatientsDataGrid.SelectionChanged += UpdateClientInfo;
            PatientSearchTextBox.TextChanged += UpdateClientsList;
            SelectDoctorButton.Click += SaveDoctorChoose;
            SaveButton.Click += CreateAppointment;
            AddPatientButton.Click += (s, e) => NavigationService.Navigate(new RegistrationClientPage());
            FindDoctorsButton.Click += async (s, e) => await FindFreeDoctors();
        }

        private void CreateAppointment(object sender, RoutedEventArgs e)
        {
            var service = ServiceComboBox.SelectedValue as ServiceDTO;
            DateTime date = (DateTime)DatePick.SelectedDate;
            DateTime endTime = date.AddMinutes(service.DurationMinutes);
            decimal totalPrice = service.BasePrice;

            if (UseBonusesCheckBox.IsChecked ?? false)
            {

            }
            using (HttpClient client = new HttpClient())
            {
                AppointmentDTO newAppointment = new AppointmentDTO
                {
                    Date = date,
                    EndTime = endTime,
                    
                };
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

            ClientNameText.Text = client.FullName;
            ClientPhoneText.Text = client.PhoneNumber;
            ClientEmailText.Text = client.Email;
            ClientNotesText.Text = client.Info;
        }

        private async Task LoadClients()
        {
            using (HttpClient client = new HttpClient())
            {
                client.AddHeaders();

                try
                {
                    var response = await client.GetAsync(ApiUrl + "clients");
                    if (response.IsSuccessStatusCode)
                    {
                        _allClients = await response.Content.ReadFromJsonAsync<List<ClientDTO>>();
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
                        MessageBox.Show("Найдено " + doctors.Count + " врачей");
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
            int serviceId = (int)ServiceComboBox.SelectedValue;
            MessageBox.Show("Выбранная услуга: " + serviceId);

            var selectedService = Services.FirstOrDefault(s => s.Id == serviceId);
            if (selectedService != null)
            {
                ServiceDescriptionText.Text = selectedService.Description;
                ServicePriceText.Text = Convert.ToString(selectedService.BasePrice) + " ₽";
                ServiceDurationText.Text = Convert.ToString(selectedService.DurationMinutes) + " минут";
            }
        }

        private async Task LoadServices()
        {
            using (HttpClient client = new HttpClient())
            {
                client.AddHeaders();

                for (int i = 0; i < EmployeeSession.ClinicsIds.Length; i++)
                {
                    var response = await client.GetAsync(ApiUrl + "clinics/services/" + EmployeeSession.ClinicsIds[i]);
                    if (response.IsSuccessStatusCode)
                    {
                        _allAvailableServices = await response.Content.ReadFromJsonAsync<List<ServiceDTO>>();
                        MessageBox.Show("Успех");
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
}
