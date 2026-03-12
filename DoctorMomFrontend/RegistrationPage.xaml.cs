using DoctorMomFrontend.Utils;
using System.Net.Http;
using System.Net.Http.Json;
using DoctorMomFrontend.Extensions;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace DoctorMomFrontend
{
    /// <summary>
    /// Логика взаимодействия для RegistrationPage.xaml
    /// </summary>
    public partial class RegistrationPage : Page
    {
        private readonly string ApiUrl = "https://localhost:7141/api/";
        public RegistrationPage()
        {
            InitializeComponent();
            RegisterButton.Click += RegisterEmployee;
            BackButton.Click += OpenMainWindow;
            RoleComboBox.SelectionChanged += CheckCurrentSelect;
            LoadClinicsAddresses();
        }

        private void OpenMainWindow(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminMainPage());
        }

        private async void LoadClinicsAddresses()
        {
            await LoadClinicsAddressesAsync();
        }

        private async Task LoadClinicsAddressesAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync(ApiUrl + "clinics/addresses");

                    string json = await response.Content.ReadAsStringAsync();
                    List<string> addresses = JsonSerializer.Deserialize<List<string>>(json);

                    ClinicComboBox.ItemsSource = addresses;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
        private void CheckCurrentSelect(object s, RoutedEventArgs e)
        {
            var selection = RoleComboBox.SelectedItem as ComboBoxItem;
            if (selection == null || selection.Tag == null)
                return;

            if (selection.Tag.ToString() == "role_doctor")
            {
                SpecializationStackPanel.Visibility = Visibility.Visible;
            }
            else
            {
                SpecializationStackPanel.Visibility = Visibility.Collapsed;
            }
        }           
        private async void RegisterEmployee(object s, RoutedEventArgs e)
        {
            var roleSelectedItem = RoleComboBox.SelectedItem as ComboBoxItem;

            string? roleContent = roleSelectedItem?.Content?.ToString();
            string? specializatiionContent = SpecializationComboBox.SelectedValue?.ToString();
            string clinicAddress;

            if (string.IsNullOrWhiteSpace(FirstNameBox.Text))
            {
                MessageBox.Show("Введите имя");
                return;
            }

            if (string.IsNullOrWhiteSpace(LoginBox.Text))
            {
                MessageBox.Show("Введите логин");
                return;
            }

            if (!int.TryParse(AgeBox.Text, out int age) || age < 18 || age > 100)
            {
                MessageBox.Show("Некорректный возраст");
                return;
            }

            if (!int.TryParse(SalaryBox.Text, out int salary) || salary < 0)
            {
                MessageBox.Show("Некорректная зарплата");
                return;
            }
            if (roleSelectedItem == null || roleSelectedItem.Tag == null)
            {
                MessageBox.Show("Выберите роль");
                return;
            }

            using (HttpClient client = new HttpClient())
            {
                client.AddHeaders();
                try
                {
                    var employeeTableDTO = new EmployeeTableDTO
                    {
                        FirstName = FirstNameBox.Text,
                        SecondName = SecondNameBox.Text,
                        PhoneNumber = PhoneBox.Text,
                        Specialization = (SpecializationStackPanel.Visibility == Visibility.Collapsed) 
                            ? roleContent : specializatiionContent,
                        Age = Int32.Parse(AgeBox.Text),
                        Salary = Int32.Parse(SalaryBox.Text),
                        Experience = Int32.Parse(ExperienceBox.Text),
                        Info = InfoBox.Text,
                        DbUsername = LoginBox.Text
                    };

                    clinicAddress = ClinicComboBox.SelectedItem as string;

                    var databaseUserDTO = new DatabaseUserDTO
                    {
                        DbUsername = LoginBox.Text,
                        DbPassword = PassBox.Password,
                        Role = roleSelectedItem.Tag.ToString()
                    };

                    RegistrationUserDTO message = new RegistrationUserDTO
                    {
                        EmployeeTableDTO = employeeTableDTO,
                        DatabaseUserDTO = databaseUserDTO,
                        ClinicLocation = clinicAddress
                    };

                    
                    var request = await client.PostAsJsonAsync(ApiUrl + "auth/register", message);

                    if (request.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Сотрудник зарегистрирован");
                        NavigationService.Navigate(new AdminMainPage());
                    }
                    else
                    {
                        string errorContent = await request.Content.ReadAsStringAsync();
                        MessageBox.Show(errorContent + " ошибка ответа");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString() + "asa");
                }                
            }
        }
    }
}
