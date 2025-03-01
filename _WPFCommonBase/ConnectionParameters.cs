﻿using System.Net;

namespace _WPFCommonBase
{
    public class ConnectionParameters
    {
        public enum Protocol
        {
            TCP,
            UDP
        }

        public IPAddress IP { get; set; }
        public int Port { get; set; }
        public Protocol SelectedProtocol { get; set; }
        public IPEndPoint Endpoint { get; set; }
        public bool WasForcedFroumOutside { get; set; }
    }
}
