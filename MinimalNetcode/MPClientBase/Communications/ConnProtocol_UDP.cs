using MPClientBase.DTO;
using System.Net;
using System.Net.Sockets;

namespace MPClientBase.Communications
{
    internal class ConnProtocol_UDP : AConnectionProtocol
    {
        public override ConnType ConnectionType => ConnType.UDP;

        private UdpClient _udpClient;

        public override void EstablishConnection(IPEndPoint ipEndPoint)
        {
            Disconnect();
            _endPoint = ipEndPoint;
            _udpClient = new UdpClient();
        }

        public override void Disconnect()
        {
            _udpClient?.Dispose();
            _udpClient = null;
        }

        public override void Send(PacketBase packet)
        {
            _udpClient.Send(packet.GetBytes(), _endPoint);
        }

        public override PacketBase Receive()
        {
            if (_udpClient.Available <= 0)
                return null;

            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            var allData = _udpClient.Receive(ref sender);

            var packetLength = BitConverter.ToUInt16(allData, 2);
            var packetData = new byte[packetLength];
            if (packetLength > 0)
                Buffer.BlockCopy(allData, 4, packetData, 0, packetLength);

            return ClientPacket.Construct((PacketType)allData[0], packetData);
        }
    }
}
