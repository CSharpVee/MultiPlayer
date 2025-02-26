using MPClientBase.DTO;
using MPModuleBase.Communications;
using PongServer.DTOs;
using PongServer.DTOs.Base;

namespace PongServer.Netcode
{
    internal class NetworkTrafficProcessor
    {
        public void RequestConfirmReady(List<ActiveConnection> connections, byte syncValue)
        {
            var packet = new MPServerToClient<EmptySendDTO>(ServerSendPurpose.Start, new EmptySendDTO());
            var serverPacket = ServerPacket.Construct(syncValue, packet);
            foreach (var conn in connections)
            {
                conn.SendPacket(serverPacket);
            }
        }

        internal void SendCountdown(List<ActiveConnection> connections, byte syncValue, CountdownState data)
        {
            var packet = new MPServerToClient<CountdownState>(ServerSendPurpose.Countdown, data);
            var serverPacket = ServerPacket.Construct(syncValue, packet);
            foreach (var conn in connections)
            {
                conn.SendPacket(serverPacket);
            }
        }

        internal void SendGamestate(List<ActiveConnection> connections, byte syncValue, GameState data)
        {
            var packet = new MPServerToClient<GameState>(ServerSendPurpose.StatusUpdate, data);
            var serverPacket = ServerPacket.Construct(syncValue, packet);
            foreach (var conn in connections)
            {
                conn.SendPacket(serverPacket);
            }
        }
    }
}
