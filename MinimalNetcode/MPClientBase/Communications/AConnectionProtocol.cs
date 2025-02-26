using MPClientBase.DTO;
using System.Net;

namespace MPClientBase.Communications
{
    public enum ConnType
    {
        TCP = 1,
        UDP
    }

    internal abstract class AConnectionProtocol
    {
        public abstract ConnType ConnectionType { get; }

        protected IPEndPoint _endPoint;

        public abstract void EstablishConnection(IPEndPoint ipEndPoint);
        public abstract void Disconnect();
        public abstract void Send(PacketBase packet);
        public abstract PacketBase Receive();
    }
}
