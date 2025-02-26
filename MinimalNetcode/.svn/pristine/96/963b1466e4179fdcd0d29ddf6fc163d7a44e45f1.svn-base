using MPModuleBase.Communications;
using MPModuleBase.Module.Base;
using MPModuleBase.Utilities;

namespace MPModuleBase.Module
{
    public abstract class AHotswappableModule : AModule
    {
        protected AHotswappableModule(LogHandle logger) : base(logger)
        {
        }

        protected override void HandleInactivity(ActiveConnection conn)
        {
            if ((DateTime.UtcNow - conn.LastContact).TotalSeconds > 20)
            {
                conn.HasHeartbeat = false;
                _connections.Remove(conn);
            }
        }

        protected sealed override void ProcessReconnect(ActiveConnection conn, byte[] data)
        {
            throw new InvalidOperationException("Reconnection attempted in a non-reconnecting module");
        }
    }
}
