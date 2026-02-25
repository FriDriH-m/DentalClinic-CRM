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
        private string connectionString = "Host=localhost;Port=5432;Database=service_center_db;Username=postgres;Password=12345;";
        public AuthorizePage()
        {
            InitializeComponent();

            ConnectToDB();
        }

        private void ConnectToDB()
        {
            try
            {
                // Используем NpgsqlConnection вместо SqlConnection
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open(); // Пытаемся подключиться
                    MessageBox.Show("Ура! Подключение к PostgreSQL успешно!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}");
            }
        }
    }
}
