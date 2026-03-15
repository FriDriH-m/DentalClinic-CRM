using DoctorMomFrontend.Extensions;
using DoctorMomFrontend.Utils;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace DoctorMomFrontend
{
    /// <summary>
    /// Логика взаимодействия для AdminServicesPage.xaml
    /// </summary>
    public partial class AdminServicesPage : Page
    {
        private List<ServiceDTO> _services = new();
        private readonly string ApiUrl = "https://localhost:7141/api/";
        public AdminServicesPage()
        {
            InitializeComponent();
            Loaded += async (s, e) => await LoadAllServices();

            EmployyesPageButton.Click += OpenEmployeesPage;
            ClinicsPageButton.Click += OpenClinicsPage;
            ServicesPageButton.Click += OpenServicesPage;
            ClinicComboBox.SelectionChanged += UpdateServiceListClinic;
            CategoryComboBox.SelectionChanged += UpdateServiceListCategory;
            AddServiceButton.Click += OpenAddServicePage;
            OpenMaterialsPageButton.Click += OpenMaterialsPage;
        }

        private void OpenMaterialsPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminMaterialsPage());
        }

        private void OpenAddServicePage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegistrationServicePage());
        }

        private void ApplyFilters()
        {
            var selectedClinic = ClinicComboBox.SelectedItem as string;
            var selectedCategory = CategoryComboBox.SelectedItem as string;

            var filteredServices = _services
                .Where(s => string.IsNullOrEmpty(selectedClinic) || s.ClinicAddress == selectedClinic)
                .Where(s => string.IsNullOrEmpty(selectedCategory) || s.CategoryName == selectedCategory)
                .ToList();

            ServicesListBox.ItemsSource = filteredServices;
        }
        private async void DeleteServiceButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var selectedService = button.DataContext as ServiceDTO;

            if (selectedService != null)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить {selectedService.Name}  id: {selectedService.Id}?",
                                             "Подтверждение", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.AddHeaders();

                        var response = await client.DeleteAsync(ApiUrl + "clinics/services/" + selectedService.Id);
                        if (response.IsSuccessStatusCode)
                        {
                            MessageBox.Show("Услуга удалена");
                        }
                        else 
                        {
                            var error = await response.Content.ReadAsStringAsync();
                            MessageBox.Show(error);
                        }
                    }
                }
            }
        }
        // позже добавлю возможность редактировать услугу
        private void RedactServiceButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var selectedService = button.DataContext as ServiceDTO;
        }
        private void UpdateServiceListCategory(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void UpdateServiceListClinic(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
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
        private async Task LoadAllServices()
        {
            using (HttpClient client = new HttpClient())
            {
                client.AddHeaders();

                var response = await client.GetAsync(ApiUrl + "clinics/services");

                if (response.IsSuccessStatusCode)
                {
                    _services = await response.Content.ReadFromJsonAsync<List<ServiceDTO>>();

                    ClinicComboBox.ItemsSource = _services
                        .Select(s => s.ClinicAddress)
                        .Distinct()
                        .ToList();

                    CategoryComboBox.ItemsSource = _services
                        .Select(s => s.CategoryName)
                        .Distinct()
                        .ToList();


                    ServicesListBox.ItemsSource = _services;
                }
                else
                {
                    MessageBox.Show("Не удалось загрузиться услуги");
                    return;
                }
            }
        }
    }
}
