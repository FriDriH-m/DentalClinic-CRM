using DoctorMomFrontend.Utils;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DoctorMomFrontend
{
    /// <summary>
    /// Логика взаимодействия для AuthorizePage.xaml
    /// </summary>
    public partial class AuthorizePage : Page
    {
        private readonly string ApiUrl = "https://localhost:7141/api/";
        public AuthorizePage()
        {
            InitializeComponent();

            LoginButton.Click += Login;
        }
        private async void Login(object s, RoutedEventArgs e)
        {
            await LoginAsync();
        }
        private async Task LoginAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    LoginUserDTO loginInfo = new LoginUserDTO(LoginTextBox.Text, PasswordBox.Password);

                    MessageBox.Show(LoginTextBox.Text + PasswordBox.Password);

                    var response = await client.PostAsJsonAsync(ApiUrl + "auth/login", loginInfo);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<SessionInfo>();

                        // Сохраняем в сессию
                        EmployeeSession.EmployeeId = result.EmployeeId;
                        EmployeeSession.Role = result.Role;

                        Page? targetPage = EmployeeSession.Role switch
                        {
                            "role_doctor" => new DoctorMainPage(),
                            "role_admin" => new AdminMainPage(),
                            "role_manager" => new ManagerMainPage(),
                            "role_analyst" => new AnalystMainPage(),
                            _ => null
                        };


                        NavigationService.Navigate(targetPage);
                    }
                    else MessageBox.Show(response.Content.ToString());
                }
                catch (Exception ex) 
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
    }
}
