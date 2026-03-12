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
        public ObservableCollection<ServiceDTO> Services { get; set; } = new();
        public ManagerAppointmentsPage()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += async (s, e) => await LoadServices();
        }

        private async Task LoadServices()
        {
            using (HttpClient client = new HttpClient())
            {
                client.AddHeaders();

                var allServices = new List<ServiceDTO>();

                for (int i = 0; i < EmployeeSession.ClinicsIds.Length; i++)
                {
                    var response = await client.GetAsync(ApiUrl + "clinics/services/" + EmployeeSession.ClinicsIds[i]);
                    if (response.IsSuccessStatusCode)
                    {
                        var services = await response.Content.ReadFromJsonAsync<List<ServiceDTO>>();
                        if (services != null)
                        {
                            MessageBox.Show(services.Count.ToString());
                            allServices.AddRange(services);
                        }
                        MessageBox.Show("Успех");
                    }
                    else
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        MessageBox.Show(error);
                    }
                }

                foreach (var item in allServices) Services.Add(item);
            }
        }
    }
}
