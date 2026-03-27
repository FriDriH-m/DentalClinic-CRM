using DoctorMomFrontend.Extensions;
using DoctorMomFrontend.Utils;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;

namespace DoctorMomFrontend
{
    /// <summary>
    /// Логика взаимодействия для ManagerClientsPage.xaml
    /// </summary>
    public partial class ManagerClientsPage : Page
    {
        private readonly string ApiUrl = "https://localhost:7141/api/";
        private List<ClientDTO> _allClients = new();
        public ManagerClientsPage()
        {
            InitializeComponent();
            Loaded += async (s, e) => await GetDataFromDB();
            
            LogoutButton.Click += (s, e) =>
            {
                NavigationService.Navigate(new AuthorizePage());
                EmployeeSession.Clear();
            };
            AppointmentsPageButton.Click += (s, e) => NavigationService.Navigate(new ManagerMainPage());
            NewClientButton.Click += (s, e) => NavigationService.Navigate(new RegistrationClientPage());
        }
        private async void RedactClient_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            ClientDTO client = button.DataContext as ClientDTO;

            EditClientWindow editClient = new EditClientWindow(client);
            editClient.ShowDialog();

            if (editClient.DialogResult == true)
            {
                using (HttpClient client1 = new HttpClient())
                {
                    await LoadClients(client1);
                    ClientsGrid.ItemsSource = _allClients;
                }                
            }
        }
        private async void HistoryClient_Click(object sender, RoutedEventArgs e)
        {

        }
        private async Task GetDataFromDB()
        {
            using (HttpClient client = new HttpClient())
            {
                client.AddHeaders();

                await LoadClients(client);
                InitData();
            }
        }
        private void InitData()
        {
            ClientsGrid.ItemsSource = _allClients;
        }
        private async Task LoadClients(HttpClient client)
        {
            try
            {
                var clientsResponse = await client.GetAsync(ApiUrl + "clients");
                if (clientsResponse.IsSuccessStatusCode)
                {
                    _allClients = await clientsResponse.Content.ReadFromJsonAsync<List<ClientDTO>>();
                }
                else
                {
                    MessageBox.Show("Не удалось получить ответ от сервера");
                }
            }
            catch
            {
                MessageBox.Show("Не удалось получить клиентов");
            }
        }
    }
}
