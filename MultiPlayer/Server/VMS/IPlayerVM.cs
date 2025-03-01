﻿using MPModuleBase.Module.Base;
using System.Collections.ObjectModel;

namespace MultiPlayer.Server.VMS
{
    internal interface IPlayerVM
    {
        public ObservableCollection<ServerInstanceVM> ActiveServerModules { get; }
        public IEnumerable<AModuleBlueprint> AvailableBlueprints { get; }
        public IEnumerable<string> BlueprintNameList { get; }
        
        public string LogLines { get; }

    }
}
