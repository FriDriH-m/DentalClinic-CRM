using DoctorMomFrontend.Extensions;
using DoctorMomFrontend.Utils;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DoctorMomFrontend
{
    /// <summary>
    /// Логика взаимодействия для CertificateManagementPage.xaml
    /// </summary>
    public partial class CertificateManagementPage : Page
    {
        private readonly string ApiUrl = "https://localhost:7141/api/";
        public CertificateManagementPage()
        {
            InitializeComponent();
            Loaded += async (s, e) => await LoadAllDoctors();

            EmployeesPageButton.Click += OpenEmployeesPage;
            ClinicsPageButton.Click += OpenClinicsPage;
            ServicesPageButton.Click += OpenServicesPage;
        }
        private void OpenServicesPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminServicesPage());
        }
        private void OpenClinicsPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminClinicsPage());
        }
        private void OpenEmployeesPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminMainPage());
        }
        private async Task LoadAllDoctors()
        {
            using (HttpClient client = new HttpClient())
            {
                client.AddHeaders();

                var response = await client.GetAsync(ApiUrl + "employees/doctors");

                if (response.IsSuccessStatusCode)
                {
                    var doctors = await response.Content.ReadFromJsonAsync<List<EmployeeTableDTO>>();

                    EmployeesGrid.ItemsSource = doctors;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show(error);
                }                    
            }
        }

        public async void GiveCertificate_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            EmployeeTableDTO doctor = btn.DataContext as EmployeeTableDTO;

            using (HttpClient client = new HttpClient())
            {
                client.AddHeaders();

                try
                {
                    var content = new StringContent("", Encoding.UTF8, "application/json");

                    var response = await client.PatchAsync(
                        ApiUrl + "employees/doctors/" + doctor.Id,
                        content
                    );

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Сертификат выдан!", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);

                        await LoadAllDoctors();
                    }
                    else
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Ошибка: {error}", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
