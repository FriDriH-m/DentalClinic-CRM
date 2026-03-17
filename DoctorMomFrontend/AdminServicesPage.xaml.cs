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
        private List<ServiceDTO> _allServices = new();
        private List<MaterialDTO> _allMaterials = new();
        private List<ClinicTableDTO> _allClinics = new();
        private readonly string ApiUrl = "https://localhost:7141/api/";
        public AdminServicesPage()
        {
            InitializeComponent();
            Loaded += async (s, e) => await LoadAllServices();
            Loaded += async (s, e) => await LoadClinicsAsync();
            Loaded += async (s, e) => await LoadAllMaterials();

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
            NavigationService.Navigate(new RegistrationServicePage(_allServices, _allMaterials, _allClinics));
        }
        private void ApplyFilters()
        {
            var selectedClinic = ClinicComboBox.SelectedItem as string;
            var selectedCategory = CategoryComboBox.SelectedItem as string;

            var filteredServices = _allServices
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
        private async void RedactServiceButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var selectedService = button.DataContext as ServiceDTO;

            if (selectedService == null) return;

            List<MaterialDTO> materials = new();

            using (HttpClient client = new HttpClient())
            {
                client.AddHeaders();

                var response = await client.GetAsync(ApiUrl + $"clinics/services/{selectedService.Id}/materials");

                if (response.IsSuccessStatusCode)
                {
                    materials = await response.Content.ReadFromJsonAsync<List<MaterialDTO>>();
                }
                else
                {
                    MessageBox.Show("Не удалось получить связи услуги с материалами");
                }
            }

            RedactorServiceWindow serviceWindow = new RedactorServiceWindow
            (
                service: selectedService,
                allClinics: _allClinics,
                allServices: _allServices,
                clinicId: selectedService.ClinicId,
                serviceMaterials: materials
            );

            serviceWindow.ShowDialog();

            if (serviceWindow.DialogResult == true)
            {
                await LoadAllServices();
            }
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
        private async Task LoadClinicsAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync(ApiUrl + "clinics");
                    if (response.IsSuccessStatusCode)
                    {
                        _allClinics = await response.Content.ReadFromJsonAsync<List<ClinicTableDTO>>();                        
                    }
                    else
                    {
                        MessageBox.Show("Не удалось агрузить клиники");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
        private async Task LoadAllMaterials()
        {
            using (HttpClient client = new HttpClient())
            {
                client.AddHeaders();

                var response = await client.GetAsync(ApiUrl + "materials");

                if (response.IsSuccessStatusCode)
                {
                    _allMaterials = await response.Content.ReadFromJsonAsync<List<MaterialDTO>>();
                }
                else
                {
                    MessageBox.Show("Не удалось загрузиться услуги");
                    return;
                }
            }
        }
        private async Task LoadAllServices()
        {
            using (HttpClient client = new HttpClient())
            {
                client.AddHeaders();

                var response = await client.GetAsync(ApiUrl + "clinics/services");

                if (response.IsSuccessStatusCode)
                {
                    _allServices = await response.Content.ReadFromJsonAsync<List<ServiceDTO>>();

                    ClinicComboBox.ItemsSource = _allServices
                        .Select(s => s.ClinicAddress)
                        .Distinct()
                        .ToList();

                    CategoryComboBox.ItemsSource = _allServices
                        .Select(s => s.CategoryName)
                        .Distinct()
                        .ToList();


                    ServicesListBox.ItemsSource = _allServices;
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
