using ChatWithHistory;
using MPClientBase.DTO;
using MPModuleBase.Module;
using MPModuleBase.Module.Base;
using MPModuleBase.Utilities;

namespace ChatWithHistoryServer
{
    internal class TestReconChatBlueprint : AModuleBlueprint
    {
        public override string Name => "Recon Chat serv";

        public override string GetVersion => "- over9000";

        public override string GetExpectedServerVersion => "0.48.0";

        public override int TargetFPS => 1;

        public override AModule CreateInstance(LogHandle logger)
        {
            return new TestReconnectChatModule(logger);
        }
    }

    internal class TestReconnectChatModule : AReconnectingModule
    {
        public override int MaxConnections => 14;

        private List<string> _savedLines = new List<string>();
        private bool _sendLast = false;
        private double _elapse;

        private Dictionary<int, ConnectionState> _connNames = new Dictionary<int, ConnectionState>();

        public TestReconnectChatModule(LogHandle logger) : base(logger)
        {
        }

        public override void BroadcastState()
        {
            if (_sendLast)
            {
                var packet = new HistoryChatPackage<MessageDTO>(Purpose.LastMessage, new MessageDTO(_savedLines.Last()));
                var lastMsgPacket = ServerPacket.Construct(Sync, packet);

                foreach (var connection in GetOnlyActiveConnections)
                    connection.SendPacket(lastMsgPacket);

                _sendLast = false;
            }

            var historyTransfers = _connNames.Where(x => x.Value.NeedsHistoryTransfer);
            if (historyTransfers.Any())
            {
                var packet = new HistoryChatPackage<MessageDTO>(Purpose.ChatHistory, new MessageDTO(string.Join("\n", _savedLines)));
                var historyPacket = ServerPacket.Construct(Sync, packet);

                foreach (var connection in historyTransfers)
                {
                    connection.Value.NeedsHistoryTransfer = false;
                    var conn = GetConnectionById(connection.Key);
                    conn.SendPacket(historyPacket);
                }
            }
        }

        protected override void ProcessIncomingPacket(int connID, byte[] data)
        {
            var purpose = (Purpose)data[0];//TODO: not a huge fan of this. There should be a nicer way.

            switch (purpose)
            {
                case Purpose.ChangeName:
                    var nameDTO = HistoryChatPackage<NameChangeDTO>.Parse(data);
                    SetName(connID, nameDTO.Data.NewName);
                    break;
                case Purpose.SendMessage:
                    var msgDTO = HistoryChatPackage<MessageDTO>.Parse(data);
                    SaveNewMessage(connID, msgDTO.Data.Message);
                    break;

                case Purpose.RequestChatHistory:
                    SendBackChatHistory(connID);
                    break;
            }
        }

        private void SendBackChatHistory(int connID)
        {
            _connNames[connID].NeedsHistoryTransfer = true;
        }

        protected override void UpdateInternal(double dTime)
        {
        }

        private void SetName(int connID, string newName)
        {
            ConnectionState entry;
            if (_connNames.ContainsKey(connID))
                entry = _connNames[connID];
            else entry = new ConnectionState();

            entry.AliasName = newName;
            _connNames[connID] = entry;
        }

        private void SaveNewMessage(int connID, string msg)
        {
            var name = _connNames.ContainsKey(connID) ? _connNames[connID].AliasName : "???";
            var dataStr = $"{name} : {msg}";
            _savedLines.Add(dataStr);
            _sendLast = true;
        }
    }

    //DTOs
    internal class ConnectionState
    {
        public string AliasName { get; set; }
        public bool NeedsHistoryTransfer { get; set; } = false;
    }
}
