using MPModuleBase.Module.Base;
using MultiPlayer.Server.Enums;
using MultiPlayer.Server.VMS;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MultiPlayer.Server
{
    internal class RootManager : IPlayerVM, INotifyPropertyChanged
    {
        public string Version => "0.48.0";
        public string ExternalIP { get; set; }

        public string Infotext { get; set; } = "";
        public ObservableCollection<ServerInstanceVM> ActiveServerModules => _serverVMs;

        public IEnumerable<AModuleBlueprint> AvailableBlueprints => _availableBlueprints;
        public IEnumerable<string> BlueprintNameList => _availableBlueprints.Select(x => x.Name);
        public string LogLines => Logger.Instance.GetLogtext(0);

        private ObservableCollection<ServerInstanceVM> _serverVMs;
        private ObservableCollection<ServerInstance> _activeServers;
        private IList<AModuleBlueprint> _availableBlueprints;


        public RootManager()
        {
            _activeServers = new ObservableCollection<ServerInstance>();
            _serverVMs = new ObservableCollection<ServerInstanceVM>();
        }

        public void Initialize()
        {
            Populate();
            Logger.Instance.CallbackWhenChanged(0, MainLogChanged);
            TryRetrievingExternalIP();
        }

        private async Task TryRetrievingExternalIP()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Using ipify.org to get the public IP
                    var response = await client.GetStringAsync("https://api.ipify.org");
                    ExternalIP = response.Trim();
                }
            }
            catch (HttpRequestException e)
            {
                ExternalIP = "Unknown";
            }
            OnPropertyChanged(nameof(ExternalIP));
        }

        public ServerInstance GetModule(ServerInstanceVM vm)
        {
            foreach (var inst in _activeServers)
                if (inst.IsModelVM(vm))
                    return inst;

            return null;
        }

        internal AddFailureReason CreateServerInstance(int selectedIndex, string instName, string instPort, ConnType protocol)
        {
            var moduleBlueprint = _availableBlueprints[selectedIndex];
            var name = !string.IsNullOrEmpty(instName) ? instName : moduleBlueprint.Name;
            var port = !string.IsNullOrEmpty(instPort) ? instPort : "8088";
            var ipp = "127.0.0.1";//what else would you set this field to anyway? It's your computer's ip.

            var canAdd = CanBeAdded(moduleBlueprint, int.Parse(port));
            if (canAdd != AddFailureReason.ALL_GOOD)
                return canAdd;


            var instance = new ServerInstance(IPAddress.Parse(ipp), int.Parse(port), name, protocol, moduleBlueprint);
            _activeServers.Add(instance);
            _serverVMs.Add(instance.ViewModel);

            return AddFailureReason.ALL_GOOD;
        }

        public void RemoveModule(ServerInstanceVM item)
        {
            var inst = GetModule(item);
            inst.Stop();
            _activeServers.Remove(inst);
            _serverVMs.Remove(item);
        }

        private void Populate()
        {
            _availableBlueprints = new List<AModuleBlueprint>();

            //do it by enumerating all .DLL files within "Modules" directory
            var filenames = Directory.GetFiles("Modules");
            foreach (var item in filenames)
            {
                var nomine = Path.Combine(Directory.GetCurrentDirectory(), item);

                var assm = Assembly.LoadFile(nomine);

                var entrypoint = assm.GetTypes().FirstOrDefault(x => typeof(AModuleBlueprint).IsAssignableFrom(x));
                var moduleinst = (entrypoint == null) ? null : Activator.CreateInstance(entrypoint) as AModuleBlueprint;
                if (moduleinst != null)
                    _availableBlueprints.Add(moduleinst);
            }

            //when developing, testing, so that it can be added to TestClassed folder, and easily instantiated.
            var dbgMOd = AddDebugModule();
            if (dbgMOd != null)
                _availableBlueprints.Add(dbgMOd);
        }

        private AModuleBlueprint AddDebugModule()
        {
            return new PongServer.PongBlueprint();
            return null;
        }

        private void MainLogChanged()
        {
            OnPropertyChanged(nameof(LogLines));
        }

        private AddFailureReason CanBeAdded(AModuleBlueprint moduleBlueprint, int port)
        {
            if (moduleBlueprint.GetExpectedServerVersion != Version)
                return AddFailureReason.WRONG_VERSION;

            if (_activeServers.Any(x => x.Port == port))
                return AddFailureReason.PORT_IN_USE;

            return AddFailureReason.ALL_GOOD;
        }

        private void SetInfotext(string text)
        {
            Infotext = text;
            OnPropertyChanged(nameof(Infotext));
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        } 
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
