using DoctorMomFrontend.Extensions;
using DoctorMomFrontend.Utils;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;
using System.Xaml;


namespace DoctorMomFrontend
{
    /// <summary>
    /// Логика взаимодействия для RegistrationServicePage.xaml
    /// </summary>
    public partial class RegistrationServicePage : Page
    {
        private ObservableCollection<MaterialDTO> _selectedMaterials;
        private readonly string ApiUrl = "https://localhost:7141/api/";
        private List<ServiceDTO> _allServices = new(); 
        private List<MaterialDTO> _allMaterials = new();
        private List<ClinicTableDTO> _allClinics = new();
        public RegistrationServicePage(
            List<ServiceDTO> allServices,
            List<MaterialDTO> allMaterials,
            List<ClinicTableDTO> allClinics)
        {
            InitializeComponent();

            _allServices = allServices;
            _allMaterials = allMaterials;
            _allClinics = allClinics;

            ClinicComboBox.ItemsSource = _allClinics;
            CategoryComboBox.ItemsSource = _allServices;

            _selectedMaterials = new();
            MaterialsListBox.ItemsSource = _selectedMaterials;

            BackButton.Click += OpenAdminServicesPage;
            AddMaterialButton.Click += AddMaterial;
            RegisterServiceButton.Click += RegisterService;
            MaterialSearchBox.TextChanged += SearchMaterial;
            ClinicComboBox.SelectionChanged += UpdateSelectedClinic;
        }
        public async void RemoveMaterialButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            MaterialDTO material = btn.DataContext as MaterialDTO;
            _selectedMaterials.Remove(material);
        }
        private async void RegisterService(object sender, RoutedEventArgs e)
        {
            if (ServiceNameBox.Text == null || 
                DescriptionBox.Text == null || 
                DurationBox.Text == null || 
                PriceBox == null || CategoryComboBox.SelectedValue == null ||
                ClinicComboBox.SelectedValue == null)
            {
                MessageBox.Show("Заполните все поля");
                return;
            }

            int countMaterials = _selectedMaterials.Count;
            Dictionary<int, int> materialsId = new();

            for (int i = 0; i < countMaterials; i++)
            {
                var currentMaterial = _selectedMaterials[i];
                materialsId.Add(currentMaterial.Id, currentMaterial.Count);
            }

            ServiceDTO newService = new ServiceDTO
            {
                Name = ServiceNameBox.Text,
                Description = DescriptionBox.Text,
                DurationMinutes = Convert.ToInt32(DurationBox.Text),
                CategoryId = (int)CategoryComboBox.SelectedValue,
                CategoryName = CategoryComboBox.Text,
                BasePrice = Convert.ToDecimal(PriceBox.Text),
                ClinicId = (int)ClinicComboBox.SelectedValue,
                ClinicAddress = ClinicComboBox.Text
            };

            var registerService = new RegisterService(newService, materialsId);

            using (HttpClient client = new HttpClient())
            {
                client.AddHeaders();
                try
                {
                    var response = await client.PostAsJsonAsync(ApiUrl + "clinics/services", registerService);
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Услугу зарегистрирована");
                    }
                    else
                    {
                        MessageBox.Show("Не удалось зарегистрировать услугу");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
        private void OpenAdminServicesPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminServicesPage());
        }

        private async void UpdateSelectedClinic(object sender, SelectionChangedEventArgs e)
        {
            MaterialsBorder.IsEnabled = ClinicComboBox.SelectedItem != null;
            MaterialsBorder.Opacity = MaterialsBorder.IsEnabled ? 1.0 : 0.5;

            _selectedMaterials.Clear();
            _allMaterials.Clear();

            var clinicId = (int)ClinicComboBox.SelectedValue;

            using (HttpClient client = new HttpClient())
            {
                client.AddHeaders();

                var response = await client.GetAsync(ApiUrl + $"materials/{clinicId}");

                if (response.IsSuccessStatusCode)
                {
                    _allMaterials = await response.Content.ReadFromJsonAsync<List<MaterialDTO>>();
                }
                else
                {
                    MessageBox.Show("Ошибка получения материалов с базы данных");
                }
            }
        }

        private async void SearchMaterial(object sender, TextChangedEventArgs e)
        {
            var searchText = MaterialSearchBox.Text.ToLower();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                return;
            }

            var filtered = _allMaterials
                .Where(m => m.Name.ToLower().Contains(searchText))
                .Take(10)
                .ToList();
        }
        private async void AddMaterial(object sender, RoutedEventArgs e)
        {
            var searchText = MaterialSearchBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                MessageBox.Show("Введите название материала для поиска", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                var material = _allMaterials
                    .FirstOrDefault(m => m.Name.ToLower() == searchText.ToLower());

                if (material == null)
                {
                    material = _allMaterials
                        .FirstOrDefault(m => m.Name.ToLower().Contains(searchText.ToLower()));
                }

                if (material == null)
                {
                    MessageBox.Show($"Материал '{searchText}' не найден", "Не найдено",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                if (_selectedMaterials.Any(m => m.Id == material.Id))
                {
                    MessageBox.Show("Этот материал уже добавлен", "Предупреждение",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _selectedMaterials.Add(new MaterialDTO
                {
                    Id = material.Id,
                    Name = material.Name,
                    Count = Convert.ToInt32(MaterialQuantityBox.Text)
                });

                MaterialSearchBox.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
