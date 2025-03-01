﻿using MPModuleBase.Module.Base;
using System.ComponentModel;

namespace MultiPlayer.Server.VMS
{
    internal class ServerInstanceVM : INotifyPropertyChanged
    {
        public string ModuleType => _mapInst.ModuleName;
        public string Name => _mapInst.Name;
        public string ModuleVersion => _mapInst.ModuleVersion;
        public int Status => _mapInst.Status;
        public string StatusShortTxt => _mapInst.StatusTxt;
        public string PlayerCounts => $"{_mapInst.ActiveConnCount}/{_mapInst.MaxServerCapacity}";
        public string Address => $"{_mapInst.IPAddress}:{_mapInst.Port}";
        public string Protocol => _mapInst.Protocol;

        public int FPSTarget => _mapInst.FPSTarget;
        public float CPU_Usage => _mapInst.CPU;
        public float RAM_Usage => _mapInst.RAM;


        public bool CanPlay => _mapInst.Status == (int)State.Unstarted || _mapInst.Status == (int)State.Crashed;
        public bool CanStop => _mapInst.Status == (int)State.Active || _mapInst.Status == (int)State.Starting;
        public bool CanPause => false;

        private IServerModelInterface _mapInst;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ServerInstanceVM(IServerModelInterface instance)
        {
            _mapInst = instance;
        }

        public void ShoutChange()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Status)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StatusShortTxt)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PlayerCounts)));

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanPlay)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanStop)));
        }
    }
}
