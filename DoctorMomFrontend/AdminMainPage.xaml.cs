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
    /// Логика взаимодействия для AdminMainPage.xaml
    /// </summary>
    public partial class AdminMainPage : Page
    {
        public ObservableCollection<EmployeeTableDTO> Employees { get; set; } = new();
        private readonly string ApiUrl = "https://localhost:7141/api/";
        public AdminMainPage()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += async (s, e) => await LoadEmployeesAsync();

            AddEmployeeButton.Click += OpenEmployeeRegistration;
            GiveEmployeeAccessButton.Click += OpenCertificateManagementPage;            
            EmployyesPageButton.Click += OpenEmployeesPage;
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
        private void OpenCertificateManagementPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CertificateManagementPage());
        }
        private void OpenEmployeeRegistration(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegistrationPage());
        }

        private async Task LoadEmployeesAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                client.AddHeaders();
                var response = await client.GetAsync(ApiUrl + "employees");

                if (response.IsSuccessStatusCode)
                {
                    var list = await response.Content.ReadFromJsonAsync<List<EmployeeTableDTO>>();
                    Employees.Clear();
                    foreach (var item in list) Employees.Add(item);
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Ошибка загрузки: {error}");
                }
            }
        }
        public async void EditButton_Click(object sender, RoutedEventArgs e)
        {

        }
        public async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var employee = button?.DataContext as EmployeeTableDTO;

            if (employee == null) return;

            using (HttpClient client = new HttpClient())
            {
                client.AddHeaders();
                var response = await client.DeleteAsync(ApiUrl + $"employees/{employee.DbUsername}");
                if (response.IsSuccessStatusCode)
                {
                    Employees.Remove(employee);
                    MessageBox.Show("Сотрудник удален из БД");
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show("Не удалось удалить сотрудника:\n" + error);
                }
            }
        }
        
    }
}
