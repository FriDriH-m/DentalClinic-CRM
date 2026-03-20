using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace DoctorMomFrontend
{
    /// <summary>
    /// Логика взаимодействия для ManagerMainPage.xaml
    /// </summary>
    public partial class ManagerMainPage : Page
    {
        public ManagerMainPage()
        {
            InitializeComponent();
            NewAppointmentButton.Click += OpenAppointmentsPage;
            NewClientButton.Click += (s, e) => NavigationService.Navigate(new RegistrationClientPage());
        }

        private void OpenAppointmentsPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ManagerAppointmentsPage());
        }
    }
}
