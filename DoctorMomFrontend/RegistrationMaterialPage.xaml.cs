using DoctorMomFrontend.Extensions;
using DoctorMomFrontend.Utils;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DoctorMomFrontend
{
    /// <summary>
    /// Логика взаимодействия для RegistrationMaterialPage.xaml
    /// </summary>
    public partial class RegistrationMaterialPage : Page
    {
        private readonly string ApiUrl = "https://localhost:7141/api/";
        public RegistrationMaterialPage()
        {
            InitializeComponent();
            Loaded += async (s, e) => await LoadClinicsAsync();

            BackButton.Click += OpenAdminMaterialsPage;
            RegisterMaterialButton.Click += RegisterMaterial;
        }

        private async void RegisterMaterial(object sender, RoutedEventArgs e)
        {
            MaterialDTO material = new MaterialDTO
            {
                Name = MaterialNameBox.Text,
                Description = DescriptionBox.Text,
                Count = int.Parse(CountTextBox.Text),
                Price = int.Parse(PriceBox.Text),
                PurchasePrice = int.Parse(PurchasePriceBox.Text),
                ClinicId = (int)ClinicComboBox.SelectedValue,
                IsCertifiedMaterial = (bool)CertifiedComboBox.SelectedValue,
            };

            using (HttpClient client = new HttpClient())
            {
                client.AddHeaders();

                var response = await client.PostAsJsonAsync(ApiUrl + "materials", material);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Материал зарегистрирован");
                }
                else
                {
                    MessageBox.Show("Не удалось зарегистрировать материал");
                    return;
                }
            }
        }

        private void OpenAdminMaterialsPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminMaterialsPage());
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
    }
}
