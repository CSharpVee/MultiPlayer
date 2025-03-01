﻿using System.Net;
using System.Windows;
using System.Windows.Controls;

namespace _WPFCommonBase
{
    /// <summary>
    /// Interaction logic for BaseConnectorControls.xaml
    /// </summary>
    /// 

    public partial class BaseConnectorControls : UserControl
    {
        public ConnectionParameters.Protocol GetProtocolSelection()
        {
            return (tcpSelector.IsChecked == true) ? ConnectionParameters.Protocol.TCP : ConnectionParameters.Protocol.UDP;
        }


        public event EventHandler<ConnectionParameters> OnJoinClicked;
        
        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(nameof(SpinnerVisible), typeof(bool), typeof(BaseConnectorControls), new PropertyMetadata(false, ChangeSpinnerVisibility));

        public bool SpinnerVisible
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        public BaseConnectorControls()
        {
            InitializeComponent();
        }

        public void ForceJoinClick()
        {
            JoinClick(null, null);
        }

        public void SetIpPort(string ipaddress, string portnumber)
        {
            ipField.Text = ipaddress;
            portField.Text = portnumber;
        }

        private void JoinClick(object sender, RoutedEventArgs e)
        {
            SpinnerVisible = true;

            var ip = IPAddress.Parse(ipField.Text);
            var port = int.Parse(portField.Text);


            var data = new ConnectionParameters()
            {
                IP = ip,
                Port = port,
                Endpoint = new IPEndPoint(ip, port),
                SelectedProtocol = GetProtocolSelection(),
                WasForcedFroumOutside = sender == null
            };

            OnJoinClicked?.Invoke(this, data);
        }

        private static void ChangeSpinnerVisibility(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (BaseConnectorControls)d;
            if ((bool)e.NewValue == true)
                control.spineroony.Visibility = Visibility.Visible;
            else control.spineroony.Visibility = Visibility.Hidden;
        }
    }
}
