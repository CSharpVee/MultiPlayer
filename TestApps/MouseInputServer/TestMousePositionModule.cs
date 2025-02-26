using MPClientBase.DTO;
using MPModuleBase.Module;
using MPModuleBase.Module.Base;
using MPModuleBase.Utilities;
using MouseInputApp.DTOs;

namespace TestMousePositionModule
{
    internal class TestMousePositionBP : AModuleBlueprint
    {
        public override string Name => "Color Cursor";
        public override string GetVersion => "0.001.1";
        public override string GetExpectedServerVersion => "0.48.0";
        public override int TargetFPS => 60;

        public override AModule CreateInstance(LogHandle logger)
        {
            return new TestMousePositionModule(logger);
        }
    }

    internal class TestMousePositionModule : AHotswappableModule
    {

        private Dictionary<int, PlayerPositionDTO> _players;

        public TestMousePositionModule(LogHandle logger) : base(logger)
        {
            _players = new Dictionary<int, PlayerPositionDTO>();
        }

        public override int MaxConnections => 10;

        public override void BroadcastState()
        {
            var length = _players.Count * (3 + 4) + 1 + 1;//3bytes color, 4 position
            var bigbuffer = new byte[length];

            bigbuffer[0] = (byte)Purpose.PositionBroadcast;
            bigbuffer[1] = (byte)_players.Count;
            var index = 0;
            foreach (var player in _players.Values)
            {
                Buffer.BlockCopy(player.Color, 0, bigbuffer, index * 7 + 2, 3);
                Buffer.BlockCopy(player.Position, 0, bigbuffer, index * 7 + 2 + 3, 4);
                index++;
            }

            var packet = ServerPacket.Construct(PacketType.NormalComms, Sync, bigbuffer);

            foreach (var connection in GetOnlyActiveConnections)
                connection.SendPacket(packet);
        }

        protected override void ProcessIncomingPacket(int connID, byte[] data)
        {
            var packet = PurpNData.Parse(data);
            switch (packet.Purpose)
            {
                case Purpose.ChangeColor:
                    SetColor(connID, packet.Data);
                    break;
                case Purpose.SendPosition:
                    SetPosition(connID, packet.Data);
                    break;
            }
        }

        private void SetPosition(int connID, byte[] data)
        {
            if (!_players.ContainsKey(connID))
                InitToDefault(connID);
            _players[connID].Position = data;
        }

        private void SetColor(int connID, byte[] color)
        {
            if (!_players.ContainsKey(connID))
                InitToDefault(connID);
            _players[connID].Color = color;
        }

        private void InitToDefault(int connID)
        {
            _players[connID] = new PlayerPositionDTO() { Position = new byte[4], Color = new byte[3] };
        }

        protected override void UpdateInternal(double dTime)
        {
            //does nothing.
            //might as well cleanup dictionary to simulate some "load".
            //And do it the most roundabout way possible :D
            var ids = _connections.Select(x => x.ConnID);

            var removalList = new List<int>();

            foreach (var entry in _players)
                if (!ids.Contains(entry.Key))
                    removalList.Add(entry.Key);

            foreach (var entry in removalList)
                _players.Remove(entry);
        }
    }


    public class PlayerPositionDTO//from THIS particular server's perspective the internal data does not have to be meaningful. It's just a safekeep and broadcast essentially.
    {
        public byte[] Color;
        public byte[] Position;
    }

    public class PurpNData
    {
        public Purpose Purpose;
        public byte[] Data;

        private PurpNData()
        {
        }

        public static PurpNData Parse(byte[] data)
        {
            var purp = data[0];
            var buff = new byte[data.Length - 1];
            Buffer.BlockCopy(data, 1, buff, 0, buff.Length);

            return new PurpNData() { Purpose = (Purpose)purp, Data = buff };
        }
    }
}
