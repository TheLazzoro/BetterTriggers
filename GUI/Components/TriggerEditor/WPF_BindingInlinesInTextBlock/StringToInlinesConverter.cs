using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Documents;

namespace GUI.Components.TriggerEditor.WPF_BindingInlinesInTextBlock
{
    public class StringToInlinesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            return new List<Inline>() { new Run(value as string) };
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
