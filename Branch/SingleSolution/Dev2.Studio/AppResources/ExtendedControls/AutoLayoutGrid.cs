﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Reactive;
using System.Reactive.Linq;
using System.Diagnostics;
using Unlimited.Applications.BusinessDesignStudio;

namespace Unlimited.Framework {
    public class AutoLayoutGrid : Grid {

        public int Columns {
            get { return (int)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Columns.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register("Columns", typeof(int), typeof(AutoLayoutGrid), new UIPropertyMetadata(0, new PropertyChangedCallback(OnPropertyChangedCallback)));

        public int Rows {
            get { return (int)GetValue(RowsProperty); }
            set { SetValue(RowsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Rows.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowsProperty =
            DependencyProperty.Register("Rows", typeof(int), typeof(AutoLayoutGrid), new UIPropertyMetadata(0, new PropertyChangedCallback(OnPropertyChangedCallback)));


        public int CellHeight {
            get { return (int)GetValue(CellHeightProperty); }
            set { SetValue(CellHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CellHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellHeightProperty =
            DependencyProperty.Register("CellHeight", typeof(int), typeof(AutoLayoutGrid), new UIPropertyMetadata(0, new PropertyChangedCallback(OnPropertyChangedCallback)));



        public int CellWidth {
            get { return (int)GetValue(CellWidthProperty); }
            set { SetValue(CellWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CellWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellWidthProperty =
            DependencyProperty.Register("CellWidth", typeof(int), typeof(AutoLayoutGrid), new UIPropertyMetadata(0, new PropertyChangedCallback(OnPropertyChangedCallback)));

        private static void OnPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var test = d as AutoLayoutGrid;
            test.BindGridStructure();
        }

        public void BindGridStructure() {


                RowDefinitions.Clear();
                ColumnDefinitions.Clear();


            Observable.Range(0, Rows)
                .Subscribe<int>(c => {
                    var rowDef = new RowDefinition();
                    if (CellHeight > 0) {
                        rowDef.Height = new GridLength(CellHeight);
                    }
                    RowDefinitions.Add(rowDef); 
                });

            Observable.Range(0, Columns)
                .Subscribe<int>(c => {
                    var def = new ColumnDefinition();
                    if (CellWidth > 0) {
                        def.Width = new GridLength(CellWidth);
                    }
                    ColumnDefinitions.Add(def); 
                });
        }

        public AutoLayoutGrid() {
            RowDefinitions.Clear();
            ColumnDefinitions.Clear();
        }

    }
}