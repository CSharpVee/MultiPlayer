﻿using MPModuleBase.Utilities;
using System.Net;

namespace MPModuleBase.Module.Base
{
    public abstract class AModuleBlueprint
    {
        public abstract string Name { get; }//DLL server's name. Aka, what's running?
        public abstract string GetVersion { get; } //Dll version
        public abstract string GetExpectedServerVersion { get; } //Version that this DLL expects of the server
        public abstract int TargetFPS { get; }

        public abstract AModule CreateInstance(LogHandle logger);
    }
}
