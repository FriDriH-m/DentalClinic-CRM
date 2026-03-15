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
    /// Логика взаимодействия для AdminMaterialsPage.xaml
    /// </summary>
    public partial class AdminMaterialsPage : Page
    {
        private List<MaterialDTO> _allMaterials;
        private readonly string ApiUrl = "https://localhost:7141/api/";
        public AdminMaterialsPage()
        {
            InitializeComponent();

            Loaded += async (s, e) => await LoadAllMaterials();

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
        private async Task LoadAllMaterials()
        {
            using (HttpClient client = new HttpClient())
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
        }
    }
}
