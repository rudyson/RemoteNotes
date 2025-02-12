﻿using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FPECS.ISTK.UI.Converters;
public class EmptyCollectionToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is ICollection collection && collection.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}