﻿using System.Net;

namespace MPModuleBase.Communications
{
    public abstract class AClientConnection
    {
        public abstract IPEndPoint Endpoint { get; }
        public abstract bool IsDataAvailable { get; }
        public abstract bool IsActive { get; }

        public abstract void Close(string reason);

        //maybe this ↓↓↓ is better? so we don't expose streamless protocol behavior as with UDP
        public abstract void SendBytes(byte[] data);
        public abstract byte[] ReceiveBytes(ushort amount);
    }
}