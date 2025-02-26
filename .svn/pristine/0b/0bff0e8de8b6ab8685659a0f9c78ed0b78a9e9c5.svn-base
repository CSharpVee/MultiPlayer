using _WPFCommonBase;
using ChatWithHistory;
using MPClientBase.Communications;
using MPClientBase.DTO;
using System.Windows;
using System.Windows.Threading;

namespace ChatAppWithHistory
{
    public partial class MainWindow : Window
    {
        private Connection _connection;
        private DispatcherTimer _timer;

        public MainWindow()
        {
            InitializeComponent();

            //Message pump
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1 / 10f);
            _timer.Tick += Update;
            _timer.Start();
        }

        private void OnJoinButtonClicked(object sender, _WPFCommonBase.ConnectionParameters e)
        {
            _connection?.Disconnect();

            _connection = CreateConnectionBySelectedType(e.SelectedProtocol);

            _connection.StageChanged += OnStateChanged;
            _connection.LastMessageArrived += (string data) => { chatLines.Text += data + "\n"; };
            _connection.ChatHistoryArrived += (string data) => { chatLines.Text += data + "\n"; };

            if (!e.WasForcedFroumOutside)
                _connection.Join(e.Endpoint);
            else _connection.ReJoin(e.Endpoint, int.Parse(uidField.Text), passkeyField.Text);
        }

        private void OnReconnectClicked(object sender, RoutedEventArgs e)
        {
            connCtrls.ForceJoinClick();
        }

        private Connection CreateConnectionBySelectedType(ConnectionParameters.Protocol selectedProtocol)
        {
            Connection conn = null;
            switch (selectedProtocol)
            {
                case ConnectionParameters.Protocol.TCP:
                    conn = new Connection(ConnType.TCP);
                    break;
                case ConnectionParameters.Protocol.UDP:
                    conn = new Connection(ConnType.UDP);
                    break;
            }

            return conn;
        }

        private void Update(object? sender, EventArgs e)
        {
            _connection?.RetrieveServerData(_timer.Interval.TotalSeconds);
        }

        private void OnStateChanged(ConnectionStage newStage)
        {
            if (newStage == ConnectionStage.JoinedIn || newStage == ConnectionStage.NormalComms)
            {
                connCtrls.SpinnerVisible = false;
                uidField.Text = _connection.GetId().ToString();
                passkeyField.Text = _connection.GetPasskey();
            }

            if (newStage == ConnectionStage.JoinedIn)
                SetName();
        }

        private void SetName()
        {
            var changeReq = new HistoryChatPackage<NameChangeDTO>(Purpose.ChangeName, new NameChangeDTO(nameField.Text));
            _connection.SendData(changeReq);
        }

        private void OnSendClick(object sender, RoutedEventArgs e)
        {
            var msgSend = new HistoryChatPackage<MessageDTO>(Purpose.SendMessage, new MessageDTO(msgField.Text));
            _connection.SendData(msgSend);

            msgField.Text = "";
        }

        private void OnReqHistoryClick(object sender, RoutedEventArgs e)
        {
            var historyRequest = new HistoryChatPackage<EmptySendDTO>(Purpose.RequestChatHistory, new EmptySendDTO());
            _connection.SendData(historyRequest);
        }

        private void EnterCheck(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                OnSendClick(sender, e);
        }
    }

    public delegate void DataArrived(string data);
    public class Connection : AConnectionBase
    {
        public int GetId() => _clientID;
        public string GetPasskey() => _passkey;

        public event DataArrived LastMessageArrived;
        public event DataArrived ChatHistoryArrived;

        public Connection(ConnType connType) : base(connType)
        {
        }

        protected override void ConcreteNormalServerComms(byte[] packetData)
        {
            if (packetData != null)
            {
                var msg = HistoryChatPackage<MessageDTO>.Parse(packetData);

                switch (msg.Purpose)
                {
                    case Purpose.LastMessage:
                        LastMessageArrived?.Invoke(msg.Data.Message);
                        break;
                    case Purpose.ChatHistory:
                        ChatHistoryArrived?.Invoke(msg.Data.Message);
                        break;
                }
            }
        }
    }
}