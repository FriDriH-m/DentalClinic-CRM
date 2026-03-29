using DoctorMomFrontend.Extensions;
using DoctorMomFrontend.Utils;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace DoctorMomFrontend
{
    /// <summary>
    /// Логика взаимодействия для DoctorMainPage.xaml
    /// </summary>
    public partial class DoctorMainPage : Page
    {
        private readonly string ApiUrl = "https://localhost:7141/api/";
        public ObservableCollection<AppointmentModelView> _allAppointments = new();
        private List<AppointmentDTO> _allAppointmentsDTO = new();
        private List<ServiceDTO> _allAvailableServices = new();
        private DateTime _selectedtDate = DateTime.Today;
        private AppointmentStatus? _selectedStatus = null;
        public DoctorMainPage()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += async (s, e) => {
                await GetDataFromDB();
                await LoadAppointments();
            };

            LogoutButton.Click += (s, e) =>
            {
                NavigationService.Navigate(new AuthorizePage());
                EmployeeSession.Clear();
            };
        }
        private void AppointmentMaterialsButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            AppointmentModelView appointment = btn.DataContext as AppointmentModelView;
            AppointmentDTO appointmentDTO = _allAppointmentsDTO
                .Where(a => a.Date == appointment.Date)
                .FirstOrDefault();

            AddAppointmentMaterials window = new AddAppointmentMaterials(appointmentDTO);
            window.ShowDialog();
        }
        private void DateFilter_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DateFilter.SelectedDate == null) return;
            _selectedtDate = DateFilter.SelectedDate.Value;
            ApplyFilter();
        }
        private void StatusFilter_Checked(object sender, RoutedEventArgs e)
        {
            if (FilterAll.IsChecked == true)
            {
                _selectedStatus = null;
                ApplyFilter(); 
            }
            else if (FilterPending.IsChecked == true)
            {
                _selectedStatus = AppointmentStatus.Pending;
                ApplyFilter(); 
            }
            else if (FilterCompleted.IsChecked == true)
            {
                _selectedStatus = AppointmentStatus.Completed;
                ApplyFilter();
            }
            else if (FilterCanceled.IsChecked == true)
            {
                _selectedStatus = AppointmentStatus.Cancelled; 
                ApplyFilter(); 
            }
        }
        private void ApplyFilter()
        {
            if (_allAppointments.Count == 0) return;
            List<AppointmentModelView> appointments = _allAppointments
                .Where(a => a.Status == _selectedStatus)
                .ToList();
            appointments = appointments.Where(a => a.Date.Date == _selectedtDate.Date).ToList();
            if (_selectedStatus == null)
            {
                AppointmentsListBox.ItemsSource = _allAppointments;
                return;
            }
            AppointmentsListBox.ItemsSource = appointments;
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
        private async Task GetDataFromDB()
        {
            using (HttpClient client = new HttpClient())
            {
                client.AddHeaders();

                await LoadServices(client);
            }
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
                        var appointments = await response.Content.ReadFromJsonAsync<List<AppointmentDTO>>();
                        _allAppointmentsDTO = appointments ??= new();

                        foreach (var appointment in appointments ?? new())
                        {
                            _allAppointments.Add(
                                new AppointmentModelView
                                {
                                    ServiceName = _allAvailableServices
                                        .FirstOrDefault(s => s.Id == appointment.ServiceId)?.Name ?? "",
                                    Date = appointment.Date,
                                    Status = appointment.Status,
                                }
                            );
                        }

                        var todayAppointments = _allAppointments.Where(a => a.Date.Date == DateTime.Today).ToList();
                        AppointmentsListBox.ItemsSource = todayAppointments;
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
        public async Task ChangeAppointmentStatus(AppointmentStatus status, object sender)
        {
            Button btn = sender as Button; 
            AppointmentModelView appointment = btn.DataContext as AppointmentModelView;

            var result = MessageBox.Show(
                $"Вы уверены, что хотите изменить статус записи {appointment.Date.Hour}:{appointment.Date.Minute} {appointment.ServiceName} ?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );
            if (result == MessageBoxResult.No) return;
            if (appointment.Status > 0)
            {
                MessageBox.Show("Запись уже нельзя поменять");
                return;
            }
            if (appointment == null)
            {
                MessageBox.Show("Не удалось получить запись");
                return;
            }

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var content = new StringContent("", Encoding.UTF8, "application/json");
                    int appointmentId = _allAppointmentsDTO
                        .Where(a => a.Date == appointment.Date)
                        .Select(a => a.Id)
                        .FirstOrDefault();

                    MessageBox.Show(Convert.ToString(appointmentId));

                    var response = await client.PatchAsync(ApiUrl + "appointments/" + appointmentId + "/" + status, content);
                    if (response.IsSuccessStatusCode)
                    {
                        appointment.Status = status;
                    }
                    else
                    {
                        MessageBox.Show("Не удалось закрыть запись, проблемы с сервером");
                    }
                }
                catch
                {
                    MessageBox.Show("Не удалось закрыть запись");
                }
            }
        }
        private async void CloseAppointmentButton_Click(object sender, RoutedEventArgs e)
        {
            await ChangeAppointmentStatus(AppointmentStatus.Completed, sender);
        }        
        private async void CancelAppointmentButton_Click(object sender, RoutedEventArgs e)
        {
            await ChangeAppointmentStatus(AppointmentStatus.Cancelled, sender);
        }
    }
}
