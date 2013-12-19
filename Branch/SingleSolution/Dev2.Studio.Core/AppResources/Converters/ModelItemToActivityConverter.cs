﻿using System;
using System.Activities;
using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Dev2.Studio.Core.Activities.Utils;

namespace Dev2.Studio.Core.AppResources.Converters
{
    public class ModelItemToActivityConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                ModelItem modelItem = value as ModelItem;

                if (modelItem != null)
                {                    
                    return modelItem;
                }
            }
            return null;
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value that is produced by the binding target.</param><param name="targetType">The type to convert to.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                ModelItem modelItem = value as ModelItem;

                if(modelItem != null)
                {
                    Activity act = modelItem.GetCurrentValue() as Activity;
                    return act;
                }                
            }
            return null;
        }

        #endregion
    }
}