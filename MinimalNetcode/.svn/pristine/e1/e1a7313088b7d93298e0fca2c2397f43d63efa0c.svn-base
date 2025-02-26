using MPClientBase.DTO;
using System.Net;
using System.Net.Sockets;

namespace MPClientBase.Communications
{
    internal class ConnProtocol_TCP : AConnectionProtocol
    {
        public override ConnType ConnectionType => ConnType.TCP;

        private TcpClient _tcpClient;
        private NetworkStream _tcpStream;

        public override void EstablishConnection(IPEndPoint ipEndPoint)
        {
            Disconnect();
            _endPoint = ipEndPoint;
            _tcpClient = new TcpClient();
            _tcpClient.Connect(_endPoint);
            _tcpStream = _tcpClient.GetStream();
        }

        public override void Disconnect()
        {
            _tcpClient?.Dispose();
            _tcpClient = null;
        }

        public override void Send(PacketBase packet)
        {
            _tcpStream.Write(packet.GetBytes());
        }

        public override PacketBase Receive()
        {
            if (!_tcpStream.DataAvailable)
                return null;

            byte[] metadata = new byte[4];
            _tcpStream.Read(metadata);

            var packetLength = BitConverter.ToUInt16(metadata, 2);
            var packetData = new byte[packetLength];
            if (packetLength > 0)
                _tcpStream.Read(packetData);

            return ClientPacket.Construct((PacketType)metadata[0], packetData);
        }
    }
}
