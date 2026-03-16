using DoctorMomFrontend.Utils;
using System.Windows;

namespace DoctorMomFrontend
{
    /// <summary>
    /// Логика взаимодействия для RedactorMaterialWindow.xaml
    /// </summary>
    public partial class RedactorMaterialWindow : Window
    {
        private MaterialDTO _material;
        public RedactorMaterialWindow(MaterialDTO material)
        {
            InitializeComponent();
            _material = material;

            WriteData(material);

            CancelButton.Click += (s, e) => 
            {
                DialogResult = false;
                Close(); 
            };
            SaveButton.Click += SaveChangesAndClose;
        }

        private void SaveChangesAndClose(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Введите название", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Обновление данных
            _material.Name = NameBox.Text;
            _material.Description = DescriptionBox.Text;
            _material.Count = int.Parse(CountBox.Text);
            _material.PurchasePrice = decimal.Parse(PurchasePriceBox.Text);
            _material.Price = decimal.Parse(PriceBox.Text);
            _material.IsCertifiedMaterial = CertifiedCheckBox.IsChecked ?? false;

            DialogResult = true;
            Close();
        }

        private void WriteData(MaterialDTO material)
        {
            NameBox.Text = material.Name;
            DescriptionBox.Text = material.Description;
            CountBox.Text = Convert.ToString(material.Count);
            PurchasePriceBox.Text = Convert.ToString(material.PurchasePrice);
            PriceBox.Text = Convert.ToString(material.Price);
            CertifiedCheckBox.IsChecked = material.IsCertifiedMaterial;
        }
    }
}
