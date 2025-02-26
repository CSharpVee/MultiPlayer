using MPClientBase.DTO;
using MPModuleBase.Module;
using MPModuleBase.Module.Base;
using MPModuleBase.Utilities;
using PongServer.DTOs;
using PongServer.DTOs.Base;
using PongServer.Game;
using PongServer.Netcode;

namespace PongServer
{
    public class PongBlueprint : AModuleBlueprint
    {
        public override string Name => "Ping-Pong server";
        public override string GetVersion => "0.0.1";
        public override string GetExpectedServerVersion => "0.48.0";

        public override int TargetFPS => 60;

        public override AModule CreateInstance(LogHandle logger) => new PongServerInstance(logger);
    }

    public class PongServerInstance : AHotswappableModule
    {
        public enum MainStates
        {
            WaitingForPlayers = 1,
            Starting,
            InGame
        }

        public override int MaxConnections => 2;


        private const float KStartBroadcastCD = 2.48f;


        private ServerGameShell _shell;
        private NetworkTrafficProcessor _networkProcessor;
        private MainStates _state;

        private float _elapse = 0;
        private Player _playerL;
        private Player _playerR;

        public PongServerInstance(LogHandle logger) : base(logger)
        {
            _shell = new ServerGameShell(new PongServer.DTOs.Duplicates.RectangleF(0, 0, 1200, 700));//sync with game
            _state = MainStates.WaitingForPlayers;
            _playerL = new Player();
            _playerR = new Player();
            StateTxt = "Lobby";

            _networkProcessor = new NetworkTrafficProcessor();
        }

        public override void BroadcastState()
        {
            switch (_state)
            {
                case MainStates.WaitingForPlayers: break;
                case MainStates.Starting:
                    if (_elapse >= KStartBroadcastCD)
                    {
                        StateTxt = "Confirming";
                        _networkProcessor.RequestConfirmReady(_connections, Sync);
                        _elapse = 0;
                    }
                    break;
                case MainStates.InGame:

                    //either full-gameplay
                    //      OR
                    //  Countdown
                    if (_shell.IsPrecount)
                    {
                        var packet = _shell.ExtractPrecount();
                        _networkProcessor.SendCountdown(_connections, Sync, packet);
                    }
                    else
                    {
                        var packet = _shell.ExtractGameplay();
                        _networkProcessor.SendGamestate(_connections, Sync, packet);
                    }


                    break;
                default:
                    break;
            }
        }

        protected override void ProcessIncomingPacket(int connID, byte[] data)
        {
            var purpose = (ClientSendPurpose)data[0];
            switch (_state)
            {
                case MainStates.Starting:
                    if (purpose == ClientSendPurpose.StartConfirmed)
                    {
                        var parsed = MPClientToServer<EmptySendDTO>.Parse(data);
                        var plr = !_playerL.ReadySentIn ? _playerL : _playerR;
                        plr.ReadySentIn = true;
                        plr.ID = connID;
                    }
                    if (_playerL.ReadySentIn && _playerR.ReadySentIn)
                        StartGame();
                    break;
                case MainStates.InGame:
                    if (purpose == ClientSendPurpose.InputState)
                    {
                        var parsed = MPClientToServer<PlayerInputDTO>.Parse(data);
                        if (_playerL.ID == connID)
                            _playerL.LastInput = parsed.Data;
                        if (_playerR.ID == connID)
                            _playerR.LastInput = parsed.Data;
                    }

                    break;
            }
        }

        protected override void UpdateInternal(double dTime)
        {
            _elapse += (float)dTime;
            if (ActiveConnectionCount < MaxConnections && _state != MainStates.WaitingForPlayers)
            {
                StateTxt = "Lobby";
                _state = MainStates.WaitingForPlayers;
                _playerL = new Player();
                _playerR = new Player();
            }
            else if (_state == MainStates.WaitingForPlayers && ActiveConnectionCount == MaxConnections)
            {
                RequestReadyConfirmation();
            }

            if (_state == MainStates.InGame)
            {
                _shell.SetInput(_playerL, _playerR);
                _shell.Update(dTime);
            }
        }

        private void StartGame()
        {
            StateTxt = "In-game";
            _state = MainStates.InGame;
        }

        private void RequestReadyConfirmation()
        {
            _state = MainStates.Starting;
            _elapse = KStartBroadcastCD;
        }
    }
}
