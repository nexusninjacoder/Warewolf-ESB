﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.IO;
using System.Windows.Media.Imaging;

namespace Unlimited.Applications.BusinessDesignStudio.Activities 
{
    public class ImagePathConverter : IValueConverter 
    {
        private string imageDirectory = Directory.GetCurrentDirectory();
        public string ImageDirectory {
            get { return imageDirectory; }
            set { imageDirectory = value; }
        }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            try {
                BitmapImage image = new BitmapImage();
                if (value != null && value.ToString() != string.Empty) {
                    Uri imageUri = new Uri(value.ToString(), UriKind.RelativeOrAbsolute);

                    image = new BitmapImage();
                    image.BeginInit();
                    image.UriSource = imageUri;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();
                }
                return image;
            }
            catch {
                return new BitmapImage(); 
            }


        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }

        #endregion
    } 
}