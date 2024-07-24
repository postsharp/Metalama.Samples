﻿using System.Windows.Data;

internal class NotValueConverter : IValueConverter
{
    public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
    {
        if ( value is bool b )
        {
            return !b;
        }

        return false;
    }

    public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
    {
        if ( value is bool b )
        {
            return !b;
        }

        return true;
    }
}