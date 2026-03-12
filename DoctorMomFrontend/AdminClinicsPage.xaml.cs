using DoctorMomFrontend.Extensions;
using DoctorMomFrontend.Utils;
using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;

namespace DoctorMomFrontend
{
    /// <summary>
    /// Логика взаимодействия для AdminClinicsPage.xaml
    /// </summary>
    public partial class AdminClinicsPage : Page
    {
        public ObservableCollection<ClinicTableDTO> Clinics { get; set; } = new();
        private readonly string ApiUrl = "https://localhost:7141/api/";
        public AdminClinicsPage()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += async (s, e) => await LoadClinicsAsync();
            EmployyesPageButton.Click += LoadEmployeesPage;
            ClinicsPageButton.Click += LoadClinicsPage;

        }
        private void LoadClinicsPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminClinicsPage());
        }
        private void LoadEmployeesPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminMainPage());
        }

        public void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            
        }
        private async Task LoadClinicsAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                client.AddHeaders();

                try
                {
                    var response = await client.GetAsync(ApiUrl + "clinics");

                    if (response.IsSuccessStatusCode)
                    {
                        var clinics = await response.Content.ReadFromJsonAsync<List<ClinicTableDTO>>();

                        Clinics.Clear();
                        foreach (var clinic in clinics) Clinics.Add(clinic);
                    }
                    else
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        MessageBox.Show(error);
                    }
                }
                catch
                {
                    MessageBox.Show("Не удалось загрузить клиники");
                }
            }
        }
    }
}
