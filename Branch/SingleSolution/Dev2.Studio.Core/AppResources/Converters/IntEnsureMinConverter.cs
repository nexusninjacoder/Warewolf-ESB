﻿using System;
using System.Windows.Data;

namespace Dev2.Studio.Core.AppResources.Converters
{
    public class IntEnsureMinConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Convert from view model int property to text

            return value; // nothing to be done - this convert is about ensuring valid min input - see ConvertBack
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Convert from user-captured text to view model int property

            var minValue = GetInt(parameter);
            var intValue = GetInt(value);

            return intValue >= minValue ? intValue : minValue;
        }

        static int GetInt(object value)
        {
            if(value != null)
            {
                int intVal;
                if(int.TryParse(value.ToString(), out intVal))
                {
                    return intVal;
                }
            }
            return 0;
        }
    }
}