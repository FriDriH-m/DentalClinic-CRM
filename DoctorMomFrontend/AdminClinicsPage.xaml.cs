using DoctorMomFrontend.Extensions;
using DoctorMomFrontend.Utils;
using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
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
            AddClinicButton.Click += LoadRegistrationClinicPage;

        }

        private void LoadRegistrationClinicPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegistrationClinicPage());
        }

        private void LoadClinicsPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminClinicsPage());
        }
        private void LoadEmployeesPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminMainPage());
        }

        public async void DeleteClinicButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var clinic = button?.DataContext as ClinicTableDTO;

            if (clinic == null)
            {
                MessageBox.Show("Не удалось удалить клинику");
                return;
            }

            using (HttpClient client = new HttpClient())
            {
                client.AddHeaders();

                var response = await client.DeleteAsync(ApiUrl + $"clinics/{clinic.Id}");
                if (response.IsSuccessStatusCode)
                {
                    Clinics.Remove(clinic);
                    MessageBox.Show("Клиника удалена из БД");
                }
                else
                {
                    MessageBox.Show("Не удалось удалить клинику");
                }
            }
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
