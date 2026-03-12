using DoctorMomFrontend.Extensions;
using DoctorMomFrontend.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
