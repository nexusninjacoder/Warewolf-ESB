﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;
using Dev2.Interfaces;

namespace Dev2.Activities
{
    public class DataGridFocusTextOnLoadBehavior : Behavior<DataGrid>
    {
        private TextBox _textBox;
        private int _stealCount;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            base.OnDetaching();
        }

        protected void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            var dgr = sender as DataGrid;

            if (dgr == null)
            {
                return;
            }
           
            var toFn = dgr.Items[0] as IDev2TOFn;
            if (toFn == null || !toFn.Inserted)
            {
                return;
            }

            _textBox = GetVisualChild<TextBox>(dgr);
            if (_textBox != null)
            {
                _textBox.Loaded += intellisenseTextBox_Loaded;
            }
        }

        void intellisenseTextBox_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            _textBox.Focus();
            _textBox.LostKeyboardFocus += _textBox_LostKeyboardFocus;
        }

        void _textBox_LostKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            _textBox.Focus();
            _stealCount++;
            if (_stealCount == 1)
            {
                _textBox.LostKeyboardFocus -= _textBox_LostKeyboardFocus;
                _textBox.Loaded -= intellisenseTextBox_Loaded;
            }
        }

        public  virtual T GetVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            T child = default(T);
            if (parent == null)
            {
                return null;
            }

            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                DependencyObject v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }
    }
}