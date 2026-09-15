using System.Globalization;
using LOCATEM_DESKTOP.Helpers.Auth;

namespace LOCATEM_DESKTOP.Converters
{
    /// <summary>Converte FieldErrorState.Active (bool) em FieldStatus.Erro/Neutro para os componentes FormEntry/PasswordEntry.</summary>
    public class ErrorActiveToStatusConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            => value is true ? FieldStatus.Erro : FieldStatus.Neutro;

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }

    /// <summary>Converte um bool? (erro/sucesso/neutro do getConfirmPasswordStatus) em FieldStatus.</summary>
    public class NullableBoolToStatusConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            => value switch
            {
                true => FieldStatus.Sucesso,
                false => FieldStatus.Erro,
                _ => FieldStatus.Neutro
            };

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
