﻿using MPModuleBase.Module.Base;
using MPModuleBase.Utilities;
using MultiPlayer.Server.Communications;
using MultiPlayer.Server.Enums;
using MultiPlayer.Server.VMS;
using System.Net;

namespace MultiPlayer.Server
{
    internal interface IServerModelInterface
    {
        public string Name { get; }
        public string ModuleName { get; }
        public string ModuleVersion { get; }
        public int Status { get; }
        public string StatusTxt { get; }
        public int MaxServerCapacity { get; }
        public int FPSTarget { get; }

        public string Protocol { get; }
        public string IPAddress { get; }
        public int Port { get; }
        public int ActiveConnCount { get; }
        public float RAM { get; }
        public float CPU { get; }
    }

    internal class ServerInstance : IServerModelInterface
    {
        public string Name { get; private set; }
        public string ModuleName => _blueprint.Name;
        public string ModuleVersion => _blueprint.GetVersion;
        public int Status => (int)_srvModule.State;
        public string StatusTxt => _srvModule.StateTxt;

        public int MaxServerCapacity => _srvModule.MaxConnections;
        public int FPSTarget => _blueprint.TargetFPS;

        public string Protocol => _protocol.ToString();
        public string IPAddress => _ipAddress.ToString();
        public int Port => _port;
        public int ActiveConnCount => _srvModule.ActiveConnectionCount;
        public float RAM => _usage_RAM;
        public float CPU => _usage_CPU;

        public ServerInstanceVM ViewModel => _VM;
        public readonly double KUpdateFrequency = 0.005;//how often UI updates. We don't need 120 FPS for this.

        private bool IsActive = false;

        private IPAddress _ipAddress;
        private int _port;
        private ConnType _protocol;
        private float _usage_RAM;//MB
        private float _usage_CPU;//percentage
        private LogHandle _logHandle;

        private AModuleBlueprint _blueprint;
        private AModule _srvModule;

        //UI stuff
        private ServerInstanceVM _VM;
        private double _uiUpdateCD = -1;
        private bool _stopRequested = false;

        public ServerInstance(IPAddress inIP, int inPort, string name, ConnType protocol, AModuleBlueprint inBlueprint)
        {
            Name = name;
            _ipAddress = inIP;
            _port = inPort;
            _protocol = protocol;

            _logHandle = Logger.Instance.StartLogging(inBlueprint.Name);

            _blueprint = inBlueprint;
            _srvModule = _blueprint.CreateInstance(_logHandle);
            _srvModule.Name = name;

            _VM = new ServerInstanceVM(this);

        }

        public void Start()
        {
            _logHandle.Log("Starting...");
            if (IsActive)
                _srvModule.Stop();

            try
            {
                var newSocket = MPConnectionFactory.CreateInstance(_ipAddress, _port, _protocol);
                _srvModule.Start(newSocket);

                IsActive = true;
                var thr = new Task(async () => { await MainLoopThread(); });//exception handling done inside
                thr.Start();
                UpdateVM(KUpdateFrequency);
            }
            catch (Exception ex)
            {
                OnCrash(ex);
            }
        }

        public void Stop()
        {
            if (!IsActive)
                return;
            
            _logHandle.Log("Stopping.");
            //cannot just stop directly, because MainLoop is on a separate thread. Might screw up the states, needs to be synchronised.
            _stopRequested = true;
        }

        private async Task MainLoopThread()
        {
            var frameStart = DateTime.UtcNow;
            _logHandle.Log("Started");

            while (IsActive)
            {
                try
                {
                    _srvModule.PreProcessConnection();//quirks & new connections

                    _srvModule.ProcessPendingConnections();
                    _srvModule.ProcessActiveConnections();

                    var delta = (DateTime.UtcNow - frameStart).TotalSeconds;
                    var frameTime = 1 / (double)_blueprint.TargetFPS;

                    if (delta < frameTime)//sleep
                    {
                        var ms = (frameTime - delta) * 1000;
                        await Task.Delay((int)ms);
                        delta = frameTime;
                    }

                    _srvModule.Update(delta);

                    frameStart = DateTime.UtcNow;

                    _srvModule.BroadcastState();
                    UpdateVM(delta);

                    if (_stopRequested)
                        PerformSyncStop();
                }
                catch (Exception ex)
                {
                    OnCrash(ex);
                }
            }
        }

        internal bool IsModelVM(ServerInstanceVM vm)
        {
            return vm == _VM;
        }

        internal string GetLogFile()
        {
            return _logHandle.GetFilename();
        }

        private void PerformSyncStop()
        {
            IsActive = false;
            _srvModule.Stop();

            UpdateVM(KUpdateFrequency);

            _stopRequested = false;
        }

        private void UpdateVM(double dTime)
        {
            _uiUpdateCD -= dTime;
            if (_uiUpdateCD <= 0)
            {
                _VM?.ShoutChange();
                _uiUpdateCD = KUpdateFrequency;
            }
        }

        private void OnCrash(Exception ex)
        {
            IsActive = false;
            _logHandle.Log(ex.ToString());
            Logger.Instance.LogMain($"!!! Module {_srvModule.Name} ({_blueprint.Name}) has crashed");
            _srvModule.SetChrashed();

            UpdateVM(KUpdateFrequency);
        }
    }

    
}
