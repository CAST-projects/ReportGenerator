using CastReporting.UI.WPF.Resources.Languages;
using System;
using System.Globalization;
using System.Windows.Data;

namespace CastReporting.UI.WPF.Converter
{
    public class MessageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Messages.ResourceManager.GetString(parameter.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
