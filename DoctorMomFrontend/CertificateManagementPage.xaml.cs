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
    /// Логика взаимодействия для CertificateManagementPage.xaml
    /// </summary>
    public partial class CertificateManagementPage : Page
    {
        private readonly string ApiUrl = "https://localhost:7141/api/";
        public CertificateManagementPage()
        {
            InitializeComponent();
            Loaded += async (s, e) => await LoadAllDoctors();
        }

        private async Task LoadAllDoctors()
        {
            using (HttpClient client = new HttpClient())
            {
                client.AddHeaders();

                var response = await client.GetAsync(ApiUrl + "employees/doctors");

                if (response.IsSuccessStatusCode)
                {
                    var doctors = await response.Content.ReadFromJsonAsync<List<EmployeeTableDTO>>();

                    EmployeesGrid.ItemsSource = doctors;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show(error);
                }                    
            }
        }

        public void GiveCertificate_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
