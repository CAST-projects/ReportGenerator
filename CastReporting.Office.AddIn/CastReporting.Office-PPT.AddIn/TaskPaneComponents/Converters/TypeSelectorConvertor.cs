using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using ComponentRepository;

namespace TaskPaneComponents
{
    class TypeSelectorConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType,
          object parameter, System.Globalization.CultureInfo culture)
        {
            string param = parameter as string;
            ComponentType type = ComponentType.All;
            ComponentType val = (ComponentType)value;

            switch (param.ToLower())
            {
                case "chart": type = ComponentType.Chart; break;
                case "table": type = ComponentType.Table; break;
                case "text": type = ComponentType.Text; break;
                default: type = ComponentType.All; break;
            }

            return val == type;
        }

        public object ConvertBack(object value, Type targetType,
          object parameter, System.Globalization.CultureInfo culture)
        {
            return parameter.ToString();
        }
    }
}
