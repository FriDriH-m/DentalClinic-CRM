using DoctorMomFrontend.Utils;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Navigation;

namespace DoctorMomFrontend
{
    /// <summary>
    /// Логика взаимодействия для EditClientWindow.xaml
    /// </summary>
    public partial class EditClientWindow : Window
    {
        private readonly string ApiUrl = "https://localhost:7141/api/";
        private readonly ClientDTO _clientDTO;
        public EditClientWindow(ClientDTO client)
        {
            InitializeComponent();
            _clientDTO = client;

            CloseButton.Click += (s, e) => Close();
            CancelButton.Click += (s, e) => Close();
            SaveButton.Click += async (s, e) => await SaveClient();
            InitData();
        }

        private async Task SaveClient()
        {
            if (string.IsNullOrWhiteSpace(FirstNameBox.Text) ||
                string.IsNullOrWhiteSpace(SecondNameBox.Text) ||
                string.IsNullOrWhiteSpace(PhoneBox.Text) ||
                string.IsNullOrEmpty(EmailBox.Text))
            {
                MessageBox.Show("Заполните поля правильно", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (HttpClient client = new HttpClient())
            {
                var newClient = new ClientDTO 
                {
                    Id = _clientDTO.Id,
                    FirstName = FirstNameBox.Text,
                    SecondName = SecondNameBox.Text,
                    PhoneNumber = PhoneBox.Text,
                    Email = EmailBox.Text,
                    Info = InfoBox.Text
                };
                var response = await client.PutAsJsonAsync(ApiUrl + "clients", newClient);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Клиент изменен");
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Не удалось обновить клиента");
                }
            }
        }

        private void InitData()
        {
            FirstNameBox.Text = _clientDTO.FirstName;
            SecondNameBox.Text = _clientDTO.SecondName;
            PhoneBox.Text = _clientDTO.PhoneNumber;
            EmailBox.Text = _clientDTO.Email;
            InfoBox.Text = _clientDTO.Info;
        }
    }
}
