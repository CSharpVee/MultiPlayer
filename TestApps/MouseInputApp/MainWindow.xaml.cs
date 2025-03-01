﻿using _WPFCommonBase;
using MouseInputApp.DTOs;
using MPClientBase.Communications;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MouseInputApp
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer _timer;

        private Connection _connection;

        private Point _lastPosition;
        private SolidColorBrush _lastBrush = Brushes.Red;
        private List<Rectangle> pool = new List<Rectangle>();

        public MainWindow()
        {
            InitializeComponent();

            //Message pump
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1 / 60f);
            _timer.Tick += Update;

            colorIndic.Fill = _lastBrush;
            for (int i = 0; i < 15; i++)
            {
                var shape = new Rectangle
                {
                    Width = 5,
                    Height = 5,
                    Fill = new SolidColorBrush(Color.FromRgb(0, 0, 0))
                };
                Canvas.SetLeft(shape, -75); // Keep shapes within canvas
                Canvas.SetTop(shape, 200);
                playerCanvas.Children.Add(shape);
                pool.Add(shape);
            }

            _timer.Start();
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
            _connection.PositionsArrived += OnPositionsArrived;
            _connection.Join(e.Endpoint);
        }

        private void OnPositionsArrived(IList<PosAndColor> data)
        {
            for (int i = 0; i < data.Count; i++)
            {
                var r = pool[i];
                var entry = data[i];

                r.Fill = new SolidColorBrush(entry.Color);
                Canvas.SetLeft(r, entry.Position.X);
                Canvas.SetTop(r, entry.Position.Y);
            }
        }

        private void OnStateChanged(ConnectionStage newStage)
        {
            if (newStage == ConnectionStage.JoinedIn)
            {
                SetColorClick(null, null);
                connCtrls.SpinnerVisible = false;
            }
        }

        private void Update(object? sender, EventArgs e)
        {
            if (_connection != null)
            {
                _connection.RetrieveServerData(_timer.Interval.TotalSeconds);

                if (_connection.Stage == ConnectionStage.NormalComms)
                    SendLatestPosition();
            }
        }


        private void SetColorClick(object sender, RoutedEventArgs e)
        {
            var rgb = new byte[] { _lastBrush.Color.R, _lastBrush.Color.G, _lastBrush.Color.B };

            var data = new MouseMovePacket<ColorData>(Purpose.ChangeColor, new ColorData(_lastBrush.Color));
            _connection.SendData(data);
        }

        private void SendLatestPosition()
        {
            //0->400; 0->400

            var data = new MouseMovePacket<PositionData>(Purpose.SendPosition, new PositionData(_lastPosition));
            _connection.SendData(data);
        }

        private void OnColorChanging(object s, System.Windows.Input.MouseButtonEventArgs e)
        {
            var sender = s as Image;
            var pos = e.GetPosition(sender);

            var center = new Point(sender.Width / 2, sender.Height / 2);
            var diff = pos - center;

            var angle = HelpCalc.CalcAngle2V(new Vector(1, 0), diff);
            if (angle < 0)
                angle = Math.PI * 2 + angle;
            var dist = diff.Length / (sender.Width / 2);
            if (dist > 1)
                dist = 1;

            _lastBrush = HelpCalc.ColorCalc(dist * 100, angle / Math.PI * 180, 100);
            colorIndic.Fill = _lastBrush;
        }

        private void OnMouseMoved(object s, System.Windows.Input.MouseEventArgs e)
        {
            var sender = s as Canvas;
            var pos = e.GetPosition(sender);

            _lastPosition = pos;
        }
    }


    delegate void PlayerPositionsArrived(IList<PosAndColor> data);
    class Connection : AConnectionBase
    {
        public event PlayerPositionsArrived PositionsArrived;

        public Connection(ConnType connType) : base(connType)
        {
        }

        protected override void ConcreteNormalServerComms(byte[] packetData)
        {
            ProcessPositions(packetData);
        }

        private void ProcessPositions(byte[] packetData)
        {
            var data = MouseMovePacket<PlayerPositions>.Parse(packetData);
            PositionsArrived?.Invoke(data.Data.Positions);
        }
    }
}