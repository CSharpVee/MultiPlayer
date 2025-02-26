using MPModuleBase.Communications;
using MPModuleBase.Module.Base;
using MPModuleBase.Utilities;
using System.Text;

namespace MPModuleBase.Module
{
    public abstract class AReconnectingModule : AModule
    {
        protected AReconnectingModule(LogHandle logger) : base(logger)
        {
        }

        protected override void HandleInactivity(ActiveConnection conn)
        {
            var diff = (DateTime.UtcNow - conn.LastContact).TotalSeconds;

            if (diff > 10)
            {
                conn.HasHeartbeat = false;
                if (diff > 5 * 60)//after 5 minutes no contact
                    _connections.Remove(conn);
            }
        }

        protected sealed override void ProcessReconnect(ActiveConnection conn, byte[] data)
        {
            int IDClaim = BitConverter.ToInt32(data, 0);
            var passkey = Encoding.ASCII.GetString(data, 4, data.Length - 4);

            var hit = _connections.FirstOrDefault(x => x.ConnID == IDClaim);
            var replaced = false;
            if (hit != null && !hit.HasHeartbeat && hit.Passkey == passkey)//this essentially prevents reconnecting until heartbeat declares connection dead. I think it's okay..?
            {
                //replace
                if (!conn.IsSame(hit))//if IP address changed, otherwise it might reuse the connection.
                    hit.Stop("Replaced");

                _connections.Remove(hit);

                conn.ConfirmConnection(IDClaim, passkey);
                conn.LastContact = DateTime.UtcNow;
                _connections.Add(conn);
                replaced = true;
            }

            if (!replaced)
                conn.Stop("Could not reconnect");

            //remove
            _pendingConnections.Remove(conn);
            SendGreetingMsg(conn, "Welcome back");
        }
    }
}
