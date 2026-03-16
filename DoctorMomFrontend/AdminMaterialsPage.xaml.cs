using DoctorMomFrontend.Extensions;
using DoctorMomFrontend.Utils;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace DoctorMomFrontend
{
    /// <summary>
    /// Логика взаимодействия для AdminMaterialsPage.xaml
    /// </summary>
    public partial class AdminMaterialsPage : Page
    {
        private List<MaterialDTO> _allMaterials = new();
        private readonly string ApiUrl = "https://localhost:7141/api/";
        public AdminMaterialsPage()
        {
            InitializeComponent();

            Loaded += async (s, e) => await LoadAllMaterials();
            Loaded += async (s, e) => await LoadClinicsAsync();

            EmployyesPageButton.Click += OpenEmployeesPage;
            ClinicsPageButton.Click += OpenClinicsPage;
            ServicesPageButton.Click += OpenServicesPage;
            ClinicComboBox.SelectionChanged += UpdateMaterialsList;
            SearchBox.TextChanged += FindMaterial;
            AddMaterialButton.Click += OpenMaterialRegPage;
            OpenServicesPageButton.Click += OpenServicesPage;
            
        }

        private void FindMaterial(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }
        public async void RedactMaterialButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            MaterialDTO material = btn.DataContext as MaterialDTO;

            RedactorMaterialWindow materialEditor = new(material);

            if (materialEditor.ShowDialog() == true)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.AddHeaders();

                    var json = JsonSerializer.Serialize(material);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PutAsync(ApiUrl + "materials/" + material.Id, content);
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Материал обновлён", "Успех",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Не удалось обновить материал");
                        return;
                    }
                }
                await LoadAllMaterials();
            }
        }

        private async Task LoadClinicsAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync(ApiUrl + "clinics");
                    List<ClinicTableDTO> clinics = await response.Content.ReadFromJsonAsync<List<ClinicTableDTO>>();
                    ClinicComboBox.ItemsSource = clinics;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
        private void ApplyFilters()
        {
            int selectedClinic = ClinicComboBox.SelectedValue == null ? 0 : (int)ClinicComboBox.SelectedValue;
            var selectedName = SearchBox.Text as string;

            var filteredServices = _allMaterials
                .Where(s => selectedClinic == 0 || s.ClinicId == Convert.ToInt32(selectedClinic))
                .Where(s => string.IsNullOrEmpty(selectedName) || s.Name.Contains(selectedName))
                .ToList();
            
            MaterialsListBox.ItemsSource = filteredServices;
        }

        private void OpenMaterialRegPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegistrationMaterialPage());
        }

        private async void UpdateMaterialsList(object sender, SelectionChangedEventArgs e)
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
        private async Task LoadAllMaterials()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.AddHeaders();

                    var response = await client.GetAsync(ApiUrl + "materials");
                    if (response.IsSuccessStatusCode)
                    {
                        _allMaterials = await response.Content.ReadFromJsonAsync<List<MaterialDTO>>();

                        MaterialsListBox.ItemsSource = _allMaterials;
                    }
                    else
                    {
                        MessageBox.Show("Не удалось загрузить материалы");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
