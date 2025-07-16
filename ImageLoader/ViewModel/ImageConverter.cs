using System.Globalization;
using System.Windows.Data;

namespace ImageLoader.ViewModel;

/// <summary>
/// Класс конвертера для привязки к картинкам
/// </summary>
public class ImageConverter : IMultiValueConverter
{
    public string? Uri { get; set; }
    public int? Index { get; set; }
    
    // Реализация интерфейса IMultiValueConverter
    
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        return new ImageConverter
        {
            Uri = values[0]?.ToString(),
            Index = int.TryParse(values[1]?.ToString(), out int index) ? index : null
        };
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        if (value is ImageConverter converter)
        {
            return new object[] {converter.Uri, converter.Index};
        }
        return Array.Empty<object>();
    }
}