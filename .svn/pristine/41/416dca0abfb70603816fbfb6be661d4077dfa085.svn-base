using MPModuleBase.Communications;
using MultiPlayer.Server.Enums;
using System.Net;

namespace MultiPlayer.Server.Communications
{
    internal class MPConnectionFactory
    {
        public static AConnectionProtSocket CreateInstance(IPAddress ipAddress, int port, ConnType type)
        {
            switch (type)
            {
                case ConnType.TCP_IP:
                    return new TCPProtSocket(IPAddress.Any, port);
                case ConnType.UDP:
                    return new UDPProtSocket(IPAddress.Any, port);
            }

            throw new ArgumentException($"What kind of protocol is this? [{type}]");
        }
    }
}
