﻿using MPClientBase.DTO;
using System.Net;
using System.Text;
using System.Text.Json;

namespace MPClientBase.Communications
{
    public enum ConnectionStage
    {
        Unconnected = 0,
        SocketConnected,
        TryToReconnect,
        JoinRequested,
        JoinedIn,
        NormalComms,
    }

    public delegate void ConnectionStageChanged(ConnectionStage newStage);
    public abstract class AConnectionBase
    {
        public ConnectionStage Stage { get; private set; }
        public ConnectionStageChanged StageChanged;

        protected readonly float KHeartbeatCD = 1.8f;
        protected bool AutoHeartbeat = true;

        protected int _clientID;
        protected string _passkey;

        private AConnectionProtocol _protocol;

        private DateTime _lastPing;//last send

        public AConnectionBase(ConnType connType)
        {
            switch (connType)
            {
                case ConnType.TCP:
                    _protocol = new ConnProtocol_TCP();
                    break;
                case ConnType.UDP:
                    _protocol = new ConnProtocol_UDP();
                    break;
            }
        }

        public void Join(IPEndPoint endpoint)
        {
            _protocol.EstablishConnection(endpoint);
            ChangeState(ConnectionStage.SocketConnected);
        }

        public void ReJoin(IPEndPoint endpoint, int idclaim, string passkey)//reconnection
        {
            Join(endpoint);
            ChangeState(ConnectionStage.TryToReconnect);

            var idBytes = BitConverter.GetBytes(idclaim);
            
            var passkeyBytes = Encoding.ASCII.GetBytes(passkey);
            var finalbytes = idBytes.Concat(passkeyBytes).ToArray();
            var clientPacket = ClientPacket.Construct(PacketType.Reconnect, finalbytes);

            _protocol.Send(clientPacket);
        }

        public void Disconnect()
        {
            _protocol.Disconnect();
            ChangeState(ConnectionStage.Unconnected);
        }

        public void SendData(byte[] data)
        {
            var clientPacket = ClientPacket.Construct(PacketType.NormalComms, data);
            _protocol.Send(clientPacket);
            _lastPing = DateTime.UtcNow;
        }

        public void SendData(IDataPack pack)
        {
            SendData(pack.GetBytes());
        }

        public void RetrieveServerData(double dTime)
        {
            if (Stage == ConnectionStage.Unconnected)
                return;

            if (Stage < ConnectionStage.NormalComms)
                ProcessInitializationComms();
            else
            {
                PacketBase packet = null;
                do
                {
                    packet = _protocol.Receive();

                    if (packet != null)
                        ConcreteNormalServerComms(packet.PacketData);
                }
                while (packet != null);

                if (AutoHeartbeat && (DateTime.Now - _lastPing).TotalSeconds > KHeartbeatCD)
                    HeartbeatPing();
            }
        }

        protected void SendSpecial(PacketBase packet)
        {
            _protocol.Send(packet);
        }


        private void ProcessInitializationComms()
        {
            var packet = _protocol.Receive();

            if (Stage == ConnectionStage.TryToReconnect && packet != null)
            {
                ReconnectionConfirmation(packet);
                return;
            }

            if (Stage == ConnectionStage.SocketConnected)
            {
                var reqJoin = ClientPacket.Construct(PacketType.ConnectJoin, new byte[0]);
                _protocol.Send(reqJoin);
                ChangeState(ConnectionStage.JoinRequested);
            }

            if (Stage == ConnectionStage.JoinRequested && packet != null)
            {
                var packetData = Encoding.ASCII.GetString(packet.PacketData);//jsonconvert, save ID, save passkey
                StoreConnectionDetails(packetData);

                ChangeState(ConnectionStage.JoinedIn);

                AfterJoinSteps();

                ChangeState(ConnectionStage.NormalComms);
            }
        }

        private void ChangeState(ConnectionStage newStage)
        {
            Stage = newStage;
            StageChanged?.Invoke(Stage);
        }

        private void StoreConnectionDetails(string packetData)//mainly for reconnect
        {
            var initResponse = JsonSerializer.Deserialize<InitialCommsDTO>(packetData);
            _clientID = initResponse.YourID;
            _passkey = initResponse.YourPasskey;
        }

        private void HeartbeatPing()
        {
            var heartbeat = ClientPacket.Construct(PacketType.Heartbeat, new byte[] { 0 });
            _protocol.Send(heartbeat);

            _lastPing = DateTime.Now;
        }

        private void ReconnectionConfirmation(PacketBase packet)
        {
            var packetData = Encoding.ASCII.GetString(packet.PacketData);//jsonconvert, save ID, save passkey
            StoreConnectionDetails(packetData);

            ChangeState(ConnectionStage.NormalComms);
        }

        protected virtual void AfterJoinSteps() { }
        protected abstract void ConcreteNormalServerComms(byte[] packetData);
    }
}
