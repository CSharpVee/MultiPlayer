﻿using MPModuleBase.Communications;
using System.Net;
using System.Net.Sockets;

namespace MultiPlayer.Server.Communications
{
    internal class TCPProtSocket : AConnectionProtSocket
    {
        public override bool IsConnectionPending => _socket.Pending();

        private TcpListener _socket;

        public TCPProtSocket(IPAddress ipAddress, int port)
        {
            _socket = new TcpListener(ipAddress, port);
        }

        public override void Close(string reason)
        {
            _socket.Stop();
            _socket.Dispose();
        }

        public override void Start()
        {
            _socket.Start();
        }

        public override AClientConnection AcceptClientConnection()
        {
            var tcpClient = new TCP_ClientConnection(_socket.AcceptTcpClient());
            return tcpClient;
        }
    }
}
