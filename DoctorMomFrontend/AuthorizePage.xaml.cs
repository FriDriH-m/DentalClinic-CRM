using Npgsql;
using System.Windows;
using System.Windows.Controls;

namespace DoctorMomFrontend
{
    /// <summary>
    /// Логика взаимодействия для AuthorizePage.xaml
    /// </summary>
    public partial class AuthorizePage : Page
    {
       
        public AuthorizePage()
        {
            InitializeComponent();

            ConnectToDB();
        }

        private void ConnectToDB()
        {
            
        }
    }
}
