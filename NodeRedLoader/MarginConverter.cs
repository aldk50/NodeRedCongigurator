using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace NodeRedConfigurator
{
    internal class MarginConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && App.Current.MainWindow.DataContext != null)
            {
                if (double.Parse(value.ToString()) < 840)
                {
                    return new Thickness((App.Current.MainWindow.DataContext as MainWindow).viewbtns.ActualWidth + 3, 1, 1, 1);
                }
                else
                {
                    return new Thickness((App.Current.MainWindow.DataContext as MainWindow).viewbtns.ActualWidth*840 / double.Parse(value.ToString()), 1, 1, 1);
                }
            }
            else return new Thickness(1, 1, 1, 1);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
