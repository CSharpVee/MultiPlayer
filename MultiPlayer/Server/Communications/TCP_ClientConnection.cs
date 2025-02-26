using System.Net;
using System.Net.Sockets;
using MPModuleBase.Communications;

namespace MultiPlayer.Server.Communications
{
    public class TCP_ClientConnection : AClientConnection
    {
        public override IPEndPoint Endpoint => _endpoint;
        public override bool IsDataAvailable => _stream.DataAvailable;

        public override bool IsActive => _client.Connected;

        private TcpClient _client;
        private NetworkStream _stream;
        private IPEndPoint _endpoint;

        public TCP_ClientConnection(TcpClient inClient)
        {
            _client = inClient;
            _stream = _client.GetStream();
            _endpoint = _client.Client.RemoteEndPoint as IPEndPoint;
        }

        public override void Close(string reason)
        {
            _stream.Dispose();
            _client.Dispose();
        }

        public override byte[] ReceiveBytes(ushort amount)
        {
            var bytes = new byte[amount];
            _stream.Read(bytes);
            return bytes;
        }

        public override void SendBytes(byte[] data)
        {
            _stream.Write(data);
        }
    }
}
