using MPModuleBase.Communications;
using System.Net;
using System.Net.Sockets;

namespace MultiPlayer.Server.Communications
{
    internal class UDPProtSocket : AConnectionProtSocket
    {
        private UdpClient _serverInst;
        private IPEndPoint _ipEndPoint;


        private IList<ByteStash> _unclaimedPackets;

        public override bool IsConnectionPending => _unclaimedPackets.Count > 0;

        public UDPProtSocket(IPAddress ipAddress, int port)
        {
            _ipEndPoint = new IPEndPoint(ipAddress, port);
            _unclaimedPackets = new List<ByteStash>();
        }

        public override void ProcessProtocolQuirks(IEnumerable<AClientConnection> connections)
        {
            var sender = new IPEndPoint(1, 1);

            for (int i = _unclaimedPackets.Count - 1; i >= 0; i--)
            {
                var packet = _unclaimedPackets[i];

                var hash = IPHash(packet.Sender);
                var targetAC = GetTargetConnection(connections, hash);
                if (targetAC != null)
                {
                    targetAC.PushNewData(packet.Data);
                    _unclaimedPackets.RemoveAt(i);
                }
            }

            try
            {
                while (_serverInst.Available > 0)
                {
                    var data = _serverInst.Receive(ref sender);
                    var hash = IPHash(sender);

                    var entry = new ByteStash(sender, data);

                    var targetAC = GetTargetConnection(connections, hash);
                    if (targetAC == null)
                        _unclaimedPackets.Add(entry);
                    else
                    {
                        targetAC.PushNewData(data);
                    }
                }
            }
            catch (Exception ex) { }
        }

        public override void Close(string reason)
        {
            _serverInst.Dispose();
        }

        public override void Start()
        {
            //it autostarts immediately upon creation... i don't know how i feel about this.
            _serverInst = new UdpClient(_ipEndPoint);
        }

        public override AClientConnection AcceptClientConnection()
        {
            var first = _unclaimedPackets.First();

            var udpClient = new UDP_ClientConnection(_serverInst, first.Sender);
            return udpClient;
        }

        private UDP_ClientConnection GetTargetConnection(IEnumerable<AClientConnection> connections, long hash)
        {
            return connections.FirstOrDefault(x => IPHash(x.Endpoint) == hash) as UDP_ClientConnection;
        }

        private long IPHash(IPEndPoint endPoint)
        {
            return ((long)endPoint.Address.GetHashCode() << 32) | (uint)endPoint.Port;
        }

        private class ByteStash
        {
            public byte[] Data;
            public IPEndPoint Sender;
            //public long IP_Hash;

            public ByteStash(IPEndPoint inSender, byte[] inData)
            {
                Data = inData;
                Sender = inSender;
            }
        }
    }
}
