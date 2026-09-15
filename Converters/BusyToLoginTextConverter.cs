using System.Globalization;

namespace LOCATEM_DESKTOP.Converters
{
    public class BusyToLoginTextConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            => value is true ? "Entrando..." : "Entrar";

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
