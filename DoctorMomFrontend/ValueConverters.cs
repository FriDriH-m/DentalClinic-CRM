using DoctorMomFrontend.Utils;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace DoctorMomFrontend
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                AppointmentStatus.Pending => new SolidColorBrush(Color.FromRgb(245, 124, 0)),   // Orange
                AppointmentStatus.Completed => new SolidColorBrush(Color.FromRgb(67, 160, 71)), // Green
                AppointmentStatus.Cancelled => new SolidColorBrush(Color.FromRgb(229, 57, 53)), // Red
                _ => new SolidColorBrush(Color.FromRgb(120, 144, 156))
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
