using DoctorMomFrontend.Extensions;
using DoctorMomFrontend.Utils;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;

namespace DoctorMomFrontend
{
    /// <summary>
    /// Логика взаимодействия для RegistrationClinicPage.xaml
    /// </summary>
    public partial class RegistrationClinicPage : Page
    {
        private readonly string ApiUrl = "https://localhost:7141/api/";
        public RegistrationClinicPage()
        {
            InitializeComponent();
            RegisterButton.Click += RegisterClinic;
            BackButton.Click += OpenClinicMainPage;
        }

        private void OpenClinicMainPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminClinicsPage());
        }

        private async void RegisterClinic(object sender, RoutedEventArgs e)
        {
            using (HttpClient client = new HttpClient())
            {
                client.AddHeaders();

                var newClinic = new ClinicTableDTO 
                { 
                    Location = LocationBox.Text,
                    PostalCode = PostalCodeBox.Text,
                    PhoneNumber = PhoneNumberBox.Text
                };


                try
                {
                    await client.PostAsJsonAsync(ApiUrl + "clinics", newClinic);
                }
                catch 
                {
                    MessageBox.Show("Не удалось зарегистрировать клинику");
                }
            }
        }
    }
}
