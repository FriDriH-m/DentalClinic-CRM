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
    /// Логика взаимодействия для AddAppointmentMaterials.xaml
    /// </summary>
    public partial class AddAppointmentMaterials : Window
    {
        private List<MaterialDTO> _allMaterials = new();
        private ObservableCollection<MaterialDTO> _appointmentMaterials = new();
        private readonly AppointmentDTO _currentAppointment;
        private readonly string ApiUrl = "https://localhost:7141/api/";
        private decimal _priceChange = 0;
        public AddAppointmentMaterials(AppointmentDTO appointmentDTO)
        {
            InitializeComponent();
            _currentAppointment = appointmentDTO;
            CancelButton.Click += (s, e) =>
            {
                DialogResult = false;
                Close();
            };
            Loaded += async (s, e) => await LoadApointmentsMaterials();

            AddMaterialButton.Click += AddMaterial;
            SaveButton.Click += async (s,e) => await SaveChanges();
        }
        private async Task SaveChanges()
        {
            Dictionary<int, int> appointmentMaterial = new();

            foreach (var material in _appointmentMaterials)
            {
                appointmentMaterial.Add(material.Id, material.Count);
            }

            var currentAppointment = new AppointmentDTO
            {
                Id = _currentAppointment.Id,
                MaterialsId = appointmentMaterial
            };

            var changes = new AppointmentMaterialsChange(currentAppointment, _priceChange);

            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.PostAsJsonAsync(ApiUrl + "appointments/materials", changes);
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Изменения сохранены");
                    }
                    else
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        MessageBox.Show("Не удалось сохранить изменения, ошибка сервера:" + error);
                    }
                }
                catch
                {                    
                    MessageBox.Show("Не удалось сохранить");
                    DialogResult = false;
                    Close();
                }
            }
            DialogResult = true;
            Close();            
        }
        private async Task LoadApointmentsMaterials()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var appointmentsMaterialResponse = await client.GetAsync(ApiUrl + "appointments/materials/" + _currentAppointment.Id);
                    var allMaterialsResponse = await client.GetAsync(ApiUrl + "materials/" + EmployeeSession.ClinicsIds[0]);
                    if (appointmentsMaterialResponse.IsSuccessStatusCode && allMaterialsResponse.IsSuccessStatusCode)
                    {
                        var appointmentMaterials = await appointmentsMaterialResponse.Content.ReadFromJsonAsync<List<MaterialDTO>>();
                        foreach (var material in appointmentMaterials)
                        {
                            _appointmentMaterials.Add(material);
                        }

                        _allMaterials = await allMaterialsResponse.Content.ReadFromJsonAsync<List<MaterialDTO>>();
                        MaterialsListBox.ItemsSource = _appointmentMaterials;
                    }
                    else MessageBox.Show("Не удалось получить материалы со стороны сервера");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Не удалось получить материалы");
                }
            }
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

                if (_appointmentMaterials.Any(m => m.Id == material.Id))
                {
                    MessageBox.Show("Этот материал уже добавлен", "Предупреждение",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _appointmentMaterials.Add(new MaterialDTO
                {
                    Id = material.Id,
                    Name = material.Name,
                    Count = Convert.ToInt32(MaterialQuantityBox.Text)
                });

                _priceChange += (material.Price * int.Parse(MaterialQuantityBox.Text));

                MaterialSearchBox.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void RemoveMaterial_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            MaterialDTO material = btn.DataContext as MaterialDTO;
            _priceChange -= (material.Price * material.Count);
            _appointmentMaterials.Remove(material);
        }
    }
}
