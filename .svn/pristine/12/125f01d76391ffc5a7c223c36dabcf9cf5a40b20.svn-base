using _WPFCommonBase;
using MPClientBase.Communications;
using MPClientBase.DTO;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace ConnTester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer _dt;
        private Connection _connection;

        public MainWindow()
        {
            InitializeComponent();
            SetIpToLocal(null, null);

            _dt = new DispatcherTimer();
            _dt.Interval = TimeSpan.FromSeconds(0.5f);
            _dt.Tick += RefreshLines;
            _dt.Start();
        }

        private void OnJoinButtonClicked(object sender, ConnectionParameters e)
        {
            _connection?.Disconnect();

            try
            {
                switch (e.SelectedProtocol)
                {
                    case ConnectionParameters.Protocol.TCP:
                        _connection = new Connection(ConnType.TCP);
                        break;
                    case ConnectionParameters.Protocol.UDP:
                        _connection = new Connection(ConnType.UDP);
                        break;
                }

                _connection.Join(e.Endpoint);
                _connection.DataArrived += (string data) => { serverReponses.Text += data + "\n"; };

                ShowInfo("Started");
            }
            catch (Exception ex)
            {
                ShowInfo(ex.Message, Brushes.DarkRed);
            }
        }

        private void RefreshLines(object? sender, EventArgs e)
        {
            try
            {
                _connection?.RetrieveServerData(_dt.Interval.TotalSeconds);
            }
            catch (Exception ex)
            {
                ShowInfo(ex.Message, Brushes.DarkRed);
            }
        }

        private void SetIpToLocal(object sender, RoutedEventArgs e)
        {
            connCtrls.SetIpPort("127.0.0.1", "6969");
        }

        private void OnDestroyClick(object sender, RoutedEventArgs e)
        {
            _connection.Disconnect();
            ShowInfo("Closed");
        }

        private void OnSendClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var type = (PacketType)int.Parse(sendType.Text);
                var bytes = string.IsNullOrEmpty(dataToSend.Text) ? new byte[0] : Encoding.ASCII.GetBytes(dataToSend.Text);

                var packet = ClientPacket.Construct(type, bytes);
                _connection.SendSpecial(packet);
            }
            catch (Exception ex)
            {
                ShowInfo(ex.Message, Brushes.DarkRed);
            }
        }

        private void ShowInfo(string text, Brush? color = null)
        {
            if (color == null)
                color = Brushes.Black;

            infoTextField.Text = text;
            infoTextField.Foreground = color;
        }

        private void PopulateJoinin(object sender, RoutedEventArgs e)
        {
            sendType.Text = "1";
            sendLength.Text = "0";
            dataToSend.Text = "";
        }

        private void PopulateNormal(object sender, RoutedEventArgs e)
        {
            sendType.Text = "4";
            sendLength.Text = "11";
            dataToSend.Text = "Test string";
        }

        private void PopulateDisconnect(object sender, RoutedEventArgs e)
        {
            sendType.Text = "3";
            sendLength.Text = "0";
            dataToSend.Text = "";
        }

        private void PopulateReconnect(object sender, RoutedEventArgs e)
        {
            sendType.Text = "2";
            sendLength.Text = "???";
            dataToSend.Text = "???";
        }

        private void dataToSend_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            sendLength.Text = dataToSend.Text.Length.ToString();
        }
    }

    delegate void DataArrived(string data);
    class Connection : AConnectionBase
    {
        public event DataArrived DataArrived;
        public Connection(ConnType connType) : base(connType)
        {
        }

        public new void SendSpecial(PacketBase pb)
        {
            base.SendSpecial(pb);
        }

        protected override void ConcreteNormalServerComms(byte[] packetData)
        {
            if (packetData != null)
            {
                var str = Encoding.ASCII.GetString(packetData);
                DataArrived?.Invoke(str);
            }
        }
    }
}