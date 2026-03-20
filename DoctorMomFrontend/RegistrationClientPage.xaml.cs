using DoctorMomFrontend.Extensions;
using DoctorMomFrontend.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace DoctorMomFrontend
{
    /// <summary>
    /// Логика взаимодействия для RegistrationClientPage.xaml
    /// </summary>
    public partial class RegistrationClientPage : Page
    {
        private readonly string ApiUrl = "https://localhost:7141/api/";
        public RegistrationClientPage()
        {
            InitializeComponent();
            BackButton.Click += (s, e) => NavigationService.GoBack();
            RegisterButton.Click += RegisterClient_Click;
        }       

        private async void RegisterClient_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FirstNameBox.Text) ||
                string.IsNullOrWhiteSpace(SecondNameBox.Text) ||
                string.IsNullOrWhiteSpace(PhoneNumberBox.Text) ||
                string.IsNullOrWhiteSpace(EmailBox.Text))
            {
                MessageBox.Show("Заполните обязательные поля", "Ошибка");
                return;
            }

            var clientDto = new ClientDTO
            {
                FirstName = FirstNameBox.Text.Trim(),
                SecondName = SecondNameBox.Text.Trim(),
                PhoneNumber = PhoneNumberBox.Text.Trim(),
                Email = EmailBox.Text.Trim(),
                Info = InfoBox.Text.Trim()
            };

            using (HttpClient client = new HttpClient())
            {
                client.AddHeaders();
                try
                {
                    var response = await client.PostAsJsonAsync(ApiUrl + "clients/register", clientDto);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Пациент зарегистрирован!", "Успех");
                        NavigationService.GoBack();
                    }
                    else
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Ошибка: {error}");
                    }
                }
                catch
                {
                    MessageBox.Show("Не удалось зарегистрировать клиента");
                }
            }
        }
    }
}
