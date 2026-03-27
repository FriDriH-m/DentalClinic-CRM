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
    /// Логика взаимодействия для RedactorServiceWindow.xaml
    /// </summary>
    public partial class RedactorServiceWindow : Window
    {
        private ObservableCollection<MaterialDTO> _selectedMaterials = new();
        private List<ServiceDTO> _allServices = new();
        private List<MaterialDTO> _allMaterials = new();
        private List<ClinicTableDTO> _allClinics = new();
        private ServiceDTO _service;
        private int _clinicId;
        private readonly string ApiUrl = "https://localhost:7141/api/";
        public RedactorServiceWindow(
            ServiceDTO service,
            List<ServiceDTO> allServices, List<ClinicTableDTO> allClinics,
            List<MaterialDTO> serviceMaterials, int clinicId)
        {
            InitializeComponent();
            foreach(var material  in serviceMaterials)
            {
                _selectedMaterials.Add(material);
            }
            _clinicId = clinicId;
            _allClinics = allClinics;
            _allServices = allServices;
            _service = service;

            Loaded += async (s, e) => await Init();

            AddMaterialButton.Click += AddMaterial;
            CancelButton.Click += (s, e) =>
            {
                DialogResult = false;
                Close();
            };
            SaveButton.Click += async (s, e) => await SaveService();
        }

        private async Task SaveService()
        {
            if (NameBox.Text == null ||
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
                Id = _service.Id,
                Name = NameBox.Text,
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
                    var response = await client.PutAsJsonAsync(ApiUrl + "clinics/services/" + _service.Id, registerService);
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


        private async Task Init()
        {
            using (HttpClient client = new HttpClient())
            {
                client.AddHeaders();

                var response = await client.GetAsync(ApiUrl + $"materials/{_clinicId}");

                if (response.IsSuccessStatusCode)
                {
                    _allMaterials = await response.Content.ReadFromJsonAsync<List<MaterialDTO>>();
                }
                else
                {
                    MessageBox.Show("Ошибка получения материалов с базы данных");
                }
            }
            MaterialsListBox.ItemsSource = _selectedMaterials;
            CategoryComboBox.ItemsSource = _allServices;
            ClinicComboBox.ItemsSource = _allClinics;

            InitData();
        }
        public void RemoveMaterial_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            MaterialDTO material = btn.DataContext as MaterialDTO;
            _selectedMaterials.Remove(material);
        }

        private void InitData()
        {
            NameBox.Text = _service.Name;
            DescriptionBox.Text = _service.Description;
            DurationBox.Text = Convert.ToString(_service.DurationMinutes);
            PriceBox.Text = Convert.ToString(_service.BasePrice);
            ClinicComboBox.SelectedItem = _allClinics.FirstOrDefault(c => c.Id == _service.ClinicId);
            CategoryComboBox.SelectedItem = _allServices.FirstOrDefault(s => s.Id == _service.Id);
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
