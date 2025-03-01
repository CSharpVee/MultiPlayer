﻿using MPModuleBase.Communications;
using MPClientBase.DTO;
using MPModuleBase.Utilities;
using System.Text.Json;

namespace MPModuleBase.Module.Base
{
    public enum State
    {
        Unstarted = 0,
        Starting,
        Active,
        ShuttingDown,
        Crashed
    }

    public abstract class AModule
    {
        public string Name { get; set; }
        public int ActiveConnectionCount => _connections.Count;
        public abstract int MaxConnections { get; }//could be instance varied. Like different size maps, modes, etc.
        public State State { get; protected set; }
        public string StateTxt { get; protected set; }


        protected byte Sync { get; private set; } = 0;
        protected AConnectionProtSocket _socket;
        protected List<ActiveConnection> _connections;
        protected List<ActiveConnection> _pendingConnections;

        private LogHandle _logger;
        private int _lastID = 0;

        protected AModule(LogHandle logger)
        {
            _connections = new List<ActiveConnection>();
            _pendingConnections = new List<ActiveConnection>();
            _logger = logger;
        }

        //lifetime events
        public virtual void Start(AConnectionProtSocket inSocket)
        {
            _socket = inSocket;
            _socket.Start();

            State = State.Active;
        }

        public virtual void Stop()
        {
            foreach (var conn in _connections)
                conn.Stop();

            _connections.Clear();

            _socket.Close("Server close requested");
            _socket = null;

            State = State.Unstarted;
        }

        public void SetChrashed()
        {
            Stop();
            State = State.Crashed;
        }

        public void CheckForIncomingData(ActiveConnection conn)
        {
            //this probably locks up
            //async read until full stop, then provide full-whole "packet" to process?
            //Alternative 2: read as many bytes as possible; assemble until a "known end" is reached; pass further to process and react.
            while (conn.DataAvailable)
            {
                var packet = conn.ReceivePacket();

                //only from pending
                if (conn.IsPending)
                {
                    switch (packet.Type)
                    {
                        case PacketType.ConnectJoin: ProcessJoinIn(conn); break;
                        case PacketType.Reconnect: ProcessReconnect(conn, packet.PacketData); break;
                        default:
                            throw new Exception($"Invalid packet {packet.Type}");
                    }
                }
                else //only from active
                {
                    switch (packet.Type)
                    {
                        case PacketType.Heartbeat: break;//just ignore, there's no valuable data within.
                        case PacketType.Disconnect: ProcessPlayerQuit(conn); break;
                        case PacketType.NormalComms: ProcessIncomingPacket(conn.ConnID, packet.PacketData); break;
                        default:
                            throw new Exception($"Invalid packet {packet.Type}");
                    }
                }


                conn.LastContact = DateTime.UtcNow;
            }

            HandleInactivity(conn);
        }

        public void Update(double dTime)
        {
            Sync++;
            UpdateInternal(dTime);
        }

        private void ProcessJoinIn(ActiveConnection conn)
        {
            if (_connections.Count < MaxConnections)
            {
                //var client = _socket.AcceptClientConnection();
                var id = GetNextID();
                var passkey = Guid.NewGuid().ToString();
                conn.ConfirmConnection(id, passkey);
                
                _pendingConnections.Remove(conn);
                AddConnection(conn);
                SendGreetingMsg(conn, "Welcome");
            }
            else
            {
                conn.GetConnectionProvider().Close("Go away now!");
            }
        }

        protected abstract void ProcessIncomingPacket(int connID, byte[] data);
        protected abstract void ProcessReconnect(ActiveConnection conn, byte[] data);
        protected abstract void HandleInactivity(ActiveConnection conn);


        protected abstract void UpdateInternal(double dTime);

        protected ActiveConnection? GetConnectionById(int connectionID)
        {
            return _connections.FirstOrDefault(x => x.ConnID == connectionID);
        }

        protected IEnumerable<ActiveConnection> GetOnlyActiveConnections => _connections.Where(x => x.HasHeartbeat && x.IsActive);

        public abstract void BroadcastState();

        //common childdren functions
        protected int GetNextID()
        {
            return ++_lastID;
        }

        protected void Log(string message)
        {
            _logger.Log(message);
        }

        protected virtual void SendGreetingMsg(ActiveConnection conn, string msg)
        {
            var initComms = new InitialCommsDTO()
            {
                YourID = conn.ConnID,
                YourPasskey = conn.Passkey,
                MSG = msg,
            };

            var packet = ServerPacket.Construct(PacketType.ConnectJoin, Sync, JsonSerializer.Serialize(initComms));
            conn.SendPacket(packet);
        }

        protected void SendFarewellMsg(ActiveConnection conn)
        {
            var msg = $"{{\"MSG\":\"Bye bye!!\"}}";
            var packet = ServerPacket.Construct(PacketType.Disconnect, Sync, msg);
            conn.SendPacket(packet);
        }

        protected void ProcessPlayerQuit(ActiveConnection conn)
        {
            SendFarewellMsg(conn);
            conn.Stop("Bye!");
            RemoveConnection(conn);
        }

        protected void AddConnection(ActiveConnection connection)
        {
            _connections.Add(connection);
        }

        protected void RemoveConnection(ActiveConnection connection)
        {
            _connections.Remove(connection);
        }

        public void PreProcessConnection()
        {
            var connProviders = _connections.Select(x => x.GetConnectionProvider()).Concat(_pendingConnections.Select(x => x.GetConnectionProvider()));
            _socket.ProcessProtocolQuirks(connProviders);

            if (_socket.IsConnectionPending)
                ProcessConnecting();
        }

        private void ProcessConnecting()
        {
            var connection = _socket.AcceptClientConnection();
            var newAC = new ActiveConnection(connection);
            _pendingConnections.Add(newAC);
        }

        public void ProcessPendingConnections()
        {
            const float KWaitToKill = 3;
            for (int i = _pendingConnections.Count - 1; i >= 0; i--)
            {
                var conn = _pendingConnections[i];
                CheckForIncomingData(conn);
                if (conn.IsPending)//hasn't connected still
                    if ((DateTime.UtcNow - conn.LastContact).TotalSeconds > KWaitToKill)
                        _pendingConnections.RemoveAt(i);
            }
        }

        public void ProcessActiveConnections()
        {
            for (int i = _connections.Count - 1; i >= 0; i--)
            {
                var conn = _connections[i];
                CheckForIncomingData(conn);
            }
        }

    }
}
