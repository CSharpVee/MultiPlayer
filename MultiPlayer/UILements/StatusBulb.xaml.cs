﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;

namespace MultiPlayer.UILements
{
    /// <summary>
    /// Interaction logic for StatusBulb.xaml
    /// </summary>
    public partial class StatusBulb : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        public int Status
        {
            get { return (int)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }
        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register(nameof(Status), typeof(int), typeof(StatusBulb), new PropertyMetadata(0, OnColorStatePropertyChanged));


        public string StatusText
        {
            get { return (string)GetValue(StatusTextProperty); }
            set { SetValue(StatusTextProperty, value); }
        }
        public static readonly DependencyProperty StatusTextProperty = DependencyProperty.Register(nameof(StatusText), typeof(string), typeof(StatusBulb), new PropertyMetadata(""));


        //public static readonly DependencyProperty StatusColorProperty = DependencyProperty.Register("State", typeof(Color), typeof(StatusBulb), new PropertyMetadata(0));

        public Color InternalStateColor { get; private set; }

        public StatusBulb()
        {
            InitializeComponent();
            InternalStateColor = GetStateColor(Status);
        }

        private Color GetStateColor(int status)
        {
            switch(status)
            {
                case 0: return Colors.LightGray;
                case 1: return Colors.Yellow;
                case 2: return Colors.Green;
                case 3: return Colors.DarkOrange;
                case 4: return Colors.Red;
            }

            return Colors.LightGray;
        }

        private static void OnColorStatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is StatusBulb control)
            {
                control.InternalStateColor = control.GetStateColor((int)e.NewValue);
                control.PropertyChanged?.Invoke(control, new PropertyChangedEventArgs(nameof(InternalStateColor)));
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
