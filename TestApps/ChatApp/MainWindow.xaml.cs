﻿using _WPFCommonBase;
using ChatAppDTOS;
using MPClientBase.Communications;
using System.Windows;
using System.Windows.Threading;

namespace ChatApp
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer _timer;
        private Connection _connection;

        public MainWindow()
        {
            InitializeComponent();

            //Message pump
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1 / 10f);
            _timer.Tick += Update;
            _timer.Start();
        }

        private void Update(object? sender, EventArgs e)
        {
            _connection?.RetrieveServerData(_timer.Interval.TotalSeconds);//not guaranteed, but to test basic connection, it suffices
        }

        private void OnJoinButtonClicked(object sender, ConnectionParameters e)
        {
            _connection?.Disconnect();

            switch (e.SelectedProtocol)
            {
                case ConnectionParameters.Protocol.TCP:
                    _connection = new Connection(ConnType.TCP);
                    break;
                case ConnectionParameters.Protocol.UDP:
                    _connection = new Connection(ConnType.UDP);
                    break;
            }

            _connection.StageChanged += OnStateChanged;
            _connection.ChatlineArrived += (string text) => { chatLines.Text += text + "\n"; };
            _connection.Join(e.Endpoint);
        }

        private void OnStateChanged(ConnectionStage newStage)
        {
            if (newStage == ConnectionStage.JoinedIn)
            {
                connCtrls.SpinnerVisible = false;
                RequestNameChange();
            }
        }

        private void RequestNameChange()
        {
            
            var pkg = new ChatPackage<NameChangeDTO>(Purpose.ChangeName, new NameChangeDTO(nameField.Text));
            _connection.SendData(pkg);
        }

        private void OnSendClick(object sender, RoutedEventArgs e)
        {
            var pkg = new ChatPackage<MessageDTO>(Purpose.SendMessage, new MessageDTO(msgField.Text));
            _connection.SendData(pkg);

            msgField.Text = "";
        }

        private void EnterCheck(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                OnSendClick(sender, e);
        }
    }

    public delegate void NewChatlineArrived(string lineText);
    internal class Connection : AConnectionBase
    {
        public event NewChatlineArrived ChatlineArrived;
        public Connection(ConnType connType) : base(connType)
        {
        }

        protected override void ConcreteNormalServerComms(byte[] bytes)
        {
            if (bytes != null)
            {
                var msg = ChatPackage<MessageDTO>.Parse(bytes);
                ChatlineArrived?.Invoke(msg.Data.Message);
            }
        }
    }
}