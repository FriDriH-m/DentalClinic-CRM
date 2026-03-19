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
        public ObservableCollection<ServiceDTO> Services { get; set; } = new();
        public ManagerAppointmentsPage()
        {
            InitializeComponent();
            InitializeTimeComboBoxes();
            DataContext = this;
            Loaded += async (s, e) => await LoadServices();

            ServiceComboBox.SelectionChanged += UpdateServiceInfo;
            ServiceComboBox.SelectionChanged += UpdateTimeIntervals;
            FindDoctorsButton.Click += async (s, e) => await FindFreeDoctors();
            //Loaded += async (s,e) => GetDoctorsAsync();
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

        private void UpdateTimeIntervals(object sender, SelectionChangedEventArgs e)
        {
            
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
