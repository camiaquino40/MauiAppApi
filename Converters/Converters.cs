using System.Globalization;

namespace MauiApiApp.Converters
{
    
    /// devuelve true si el objeto no es null
    /// mostrar el contenido cuando Post fue cargado
  
    public class IsNotNullConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            => value is not null;

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    
    /// devuelve true si el string no está vacío
    /// mostrar el Label de error solo cuando hay mensaje
    
    public class StringNotEmptyConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            => value is string s && !string.IsNullOrEmpty(s);

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
