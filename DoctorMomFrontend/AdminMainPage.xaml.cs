using System.Windows;
using System.Windows.Controls;

namespace DoctorMomFrontend
{
    /// <summary>
    /// Логика взаимодействия для AdminMainPage.xaml
    /// </summary>
    public partial class AdminMainPage : Page
    {
        private readonly string ApiUrl = "https://localhost:7141/api/";
        public AdminMainPage()
        {
            InitializeComponent();

            AddEmployyeButton.Click += OpenEmployeeRegistration;
        }

        private void OpenEmployeeRegistration(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegistrationPage());
        }
    }
}
