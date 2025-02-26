using MPClientBase.DTO;
using MPModuleBase.Communications;
using MPModuleBase.Module;
using MPModuleBase.Module.Base;
using MPModuleBase.Utilities;

namespace ChatServer
{
    internal class TestHotswapChatBP : AModuleBlueprint
    {
        public override string Name => "MirC HS server";
        public override string GetVersion => "-1.1";
        public override string GetExpectedServerVersion => "0.48.0";
        public override int TargetFPS => 5;

        public override AModule CreateInstance(LogHandle logger)
        {
            return new TestHotswapChatModule(logger);
        }
    }

    internal class TestHotswapChatModule : AHotswappableModule
    {
        public override int MaxConnections => 50;

        private List<string> _savedLines = new List<string>();
        private bool _sendLast = false;
        private double _elapse;

        //this essentially leaks when connections drop. I know. It's just a test module. Relax! IT'S YOU WHO ARE SHOUTING!!!!
        private Dictionary<int, string> _connNames = new Dictionary<int, string>();

        public TestHotswapChatModule(LogHandle logger) : base(logger)
        {

        }

        public override void BroadcastState()
        {
            if (!_sendLast)
                return;

            var msg = new ChatAppDTOS.MessageDTO(_savedLines.Last());
            var packet = new ChatAppDTOS.ChatPackage<ChatAppDTOS.MessageDTO>(ChatAppDTOS.Purpose.MessageBroadcast, msg);
            var normalPacket = ServerPacket.Construct(Sync, packet);

            foreach (var connection in GetOnlyActiveConnections)
                connection.SendPacket(normalPacket);

            _sendLast = false;
        }

        public override void Start(AConnectionProtSocket inSocket)
        {
            base.Start(inSocket);
            _elapse = 0;
            State = State.Starting;
        }

        protected override void UpdateInternal(double dTime)
        {
            _elapse += dTime;

            if (_elapse > 1.3f)
                State = State.Active;

            StateTxt = $"{_elapse:0.##}";
        }

        protected override void ProcessIncomingPacket(int connID, byte[] data)
        {
            var purpose = (ChatAppDTOS.Purpose)data[0];//TODO: not a huge fan of this. There should be a nicer way.

            switch (purpose)
            {
                case ChatAppDTOS.Purpose.ChangeName:
                    var nameDTO = ChatAppDTOS.ChatPackage<ChatAppDTOS.NameChangeDTO>.Parse(data);
                    SetName(connID, nameDTO.Data.NewName);
                    break;
                case ChatAppDTOS.Purpose.SendMessage:
                    var msgDTO = ChatAppDTOS.ChatPackage<ChatAppDTOS.MessageDTO>.Parse(data);
                    SaveNewMessage(connID, msgDTO.Data.Message);
                    break;
            }
        }

        private void SetName(int connID, string newName)
        {
            _connNames[connID] = newName;
        }

        private void SaveNewMessage(int connID, string msg)
        {
            var name = _connNames.ContainsKey(connID) ? _connNames[connID] : "???";
            var dataStr = $"{name} : {msg}";
            _savedLines.Add(dataStr);
            _sendLast = true;
        }
    }

   
}
