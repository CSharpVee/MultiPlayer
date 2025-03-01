﻿using MultiPlayer.Server;
using MultiPlayer.Server.Enums;
using MultiPlayer.Server.VMS;
using MultiPlayer.UILements;
using System.Diagnostics;
using System.Windows;

namespace MultiPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _showLogs;
        private RootManager _root;

        public MainWindow()
        {
            InitializeComponent();
            Logger.Instance.LogMain("Starting up...");

            _root = new RootManager();
            _root.Initialize();

            rootGrid.DataContext = _root;

            _showLogs = false;
            modulesDropbox.SelectedIndex = 3;

            Logger.Instance.LogMain("Initialized");
        }

        private void PlayClicked(object sender, RoutedEventArgs e)
        {
            var item = (sender as ImageButton)?.DataContext as ServerInstanceVM;
            if (item != null)
            {
                var inst = _root.GetModule(item);
                Logger.Instance.LogMain($"Running module [{inst.ModuleName}]");
                inst.Start();
            }
        }

        private void StopClicked(object sender, RoutedEventArgs e)
        {
            var item = (sender as ImageButton)?.DataContext as ServerInstanceVM;
            if (item != null)
            {
                var inst = _root.GetModule(item);
                inst.Stop();
            }
        }

        private void PauseClicked(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void RemoveClicked(object sender, RoutedEventArgs e)
        {
            var item = (sender as ImageButton)?.DataContext as ServerInstanceVM;
            if (item != null)
                _root.RemoveModule(item);
        }

        private void AddInstanceClicked(object sender, RoutedEventArgs e)
        {
            //Pre-add checks
            var protocol = tcpCheck.IsChecked == true? ConnType.TCP_IP : ConnType.UDP;
            var result = _root.CreateServerInstance(modulesDropbox.SelectedIndex, newName.Text, newPort.Text, protocol);

            switch (result)
            {
                case Server.Enums.AddFailureReason.PORT_IN_USE:
                    addErrorTextf.Text = "Port is already in use"; break;
                case Server.Enums.AddFailureReason.WRONG_VERSION:
                    addErrorTextf.Text = "MPlayer & Module expected versions differ"; break;
                case Server.Enums.AddFailureReason.ALL_GOOD:
                    addErrorTextf.Text = ""; break;
            }
        }

        private void LogVisibilitToggleClick(object sender, RoutedEventArgs e)
        {
            _showLogs = !_showLogs;
            logsColumn.Width = new GridLength(_showLogs ? 20.0 : 0.0, GridUnitType.Star);
            this.Width = (_showLogs) ? 1667 : 1000;
        }

        private void ShowLogs(object sender, RoutedEventArgs e)
        {
            var item = (sender as ImageButton)?.DataContext as ServerInstanceVM;
            if (item != null)
            {
                var inst = _root.GetModule(item);
                var filename = inst.GetLogFile();
                new Process
                {
                    StartInfo = new ProcessStartInfo(filename)
                    {
                        UseShellExecute = true
                    }
                }.Start();
            }
        }

        private void CopyIp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Clipboard.SetText(_root.ExternalIP);
        }
    }

}