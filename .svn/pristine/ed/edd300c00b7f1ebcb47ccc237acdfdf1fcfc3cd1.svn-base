using MPClientBase.DTO;
using System.Net;

namespace MPModuleBase.Communications
{
    public class ActiveConnection
    {
        public bool HasHeartbeat { get; internal set; }//heartbeat lost
        public bool IsActive => _clientConn.IsActive;

        public bool IsPending => _id < 0;
        public AClientConnection GetConnectionProvider() => _clientConn;
        public IPEndPoint Endpoint => _clientConn.Endpoint;
        public int ConnID => _id;
        public string Passkey => _passkey;
        public bool DataAvailable => _clientConn.IsDataAvailable;
        public DateTime LastContact { get; set; }

        private int _id = -1;
        private string _passkey;//for reconnects, verification and schit
        private AClientConnection _clientConn;

        public ActiveConnection(AClientConnection inClient)
        {
            HasHeartbeat = true;

            _clientConn = inClient;

            LastContact = DateTime.UtcNow;
        }

        public void ConfirmConnection(int inID, string inPasskey)
        {
            _id = inID;
            _passkey = inPasskey;
        }

        public void Stop(string reason = null)
        {
            reason = string.IsNullOrEmpty(reason) ? "Stop requested" : reason;
            _clientConn.Close(reason);
        }

        public void SendPacket(PacketBase packet)
        {
            if (_clientConn.IsActive)
            {
                try
                {
                    _clientConn.SendBytes(packet.GetBytes());
                }
                catch (Exception ex) { }
            }
        }

        public ClientPacket ReceivePacket()
        {
            var metadata = _clientConn.ReceiveBytes(3);

            var packetLength = BitConverter.ToUInt16(metadata, 1);
            var packetData = packetLength > 0 ? _clientConn.ReceiveBytes(packetLength) : new byte[0];

            return ClientPacket.Construct((PacketType)metadata[0], packetData);
        }

        internal bool IsSame(ActiveConnection hit)
        {
            return this.Endpoint.Equals(hit.Endpoint);
        }
    }
}
