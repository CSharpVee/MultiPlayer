﻿using MPModuleBase.Communications;

namespace MPModuleBase.Communications
{
    public abstract class AConnectionProtSocket
    {
        public abstract bool IsConnectionPending { get; }

        public abstract void Start();
        public abstract void Close(string reason);//after close it becomes unusable

        public abstract AClientConnection AcceptClientConnection();
        public virtual void ProcessProtocolQuirks(IEnumerable<AClientConnection> connections)//mainly for UDP weirdness
        {
        }
    }
}
