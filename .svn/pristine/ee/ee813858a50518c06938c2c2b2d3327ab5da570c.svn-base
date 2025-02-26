using MPModuleBase.Module.Base;
using MPModuleBase.Utilities;
using MultiPlayer.Server;
using MultiPlayer.Server.VMS;
using System.Collections.ObjectModel;

namespace MultiPlayer.TestClasses
{
    //for UI quicktest
    internal class VMTest : IPlayerVM
    {
        public ObservableCollection<ServerInstanceVM> ActiveServerModules => _activeModules;
        public string LogLines => "Log line 1\nLog line 2\nSome other log line\nMany more logs\nLogs logs logs.\n...";
        public IEnumerable<AModuleBlueprint> AvailableBlueprints => _avlbBlueprints;

        public IEnumerable<string> BlueprintNameList => _avlbBlueprints.Select(x => x.Name);

        private ObservableCollection<ServerInstanceVM> _activeModules;
        private List<AModuleBlueprint> _avlbBlueprints;


        public VMTest()
        {
            _activeModules = new ObservableCollection<ServerInstanceVM>()
            {
                new ServerInstanceVM(new TestActiveServer() { ModuleName = "Dopatwo", Name = "Dota srv1", ModuleVersion = "2.8.51", Status = 0, StatusTxt = "yup", ActiveConnCount = 3, MaxServerCapacity = 10, Protocol="TCP/IP", IPAddress = "10.2.5.188", Port = 8080, CPU = 2, FPSTarget = 90, RAM = 550}),
                new ServerInstanceVM(new TestActiveServer() { ModuleName = "Dopatwo", Name = "Dota srv5", ModuleVersion = "2.8.51", Status = 1, StatusTxt = "nop", ActiveConnCount = 10, MaxServerCapacity = 10, Protocol="TCP/IP", IPAddress = "10.2.5.188", Port = 8075, CPU = 12, FPSTarget = 90, RAM = 1550}),
                new ServerInstanceVM(new TestActiveServer() { ModuleName = "mIRC", Name = "Baro servas", ModuleVersion = "1.2.185.15", Status = 2, StatusTxt = "", ActiveConnCount = 52, MaxServerCapacity = 999, Protocol="UDP", IPAddress = "10.2.5.188", Port = 6, CPU = 1, FPSTarget = 3, RAM = 15}),
                new ServerInstanceVM(new TestActiveServer() { ModuleName = "SSSnake", Name = "Gyvate", ModuleVersion = "0.95", Status = 3, StatusTxt = "", ActiveConnCount = 4, MaxServerCapacity = 4, Protocol="UDP", IPAddress = "10.2.5.188", Port = 69, CPU = 3, FPSTarget = 90, RAM = 101})
            };

            _avlbBlueprints = new List<AModuleBlueprint>()
            {
                new TestModule("Mirc", "1111"),
                new TestModule("Dopatwo", "1111"),
                new TestModule("CS 1.6", "1.6")
            };
        }
    }

    public class TestModule : AModuleBlueprint
    {
        public override string Name => _name;
        public override string GetVersion => _version;
        public override string GetExpectedServerVersion => throw new NotImplementedException();
        public override int TargetFPS => throw new NotImplementedException();

        private string _name;
        private string _version;

        public TestModule(string inName, string inVersion)
        {
            _name = inName;
            _version = inVersion;
        }

        public override AModule CreateInstance(LogHandle logger) => throw new NotImplementedException();
    }

    public class TestActiveServer : IServerModelInterface
    {
        public string ModuleName { get; set; }
        public string Name { get; set; }

        public string ModuleVersion { get; set; }

        public int Status { get; set; }

        public string StatusTxt { get; set; }

        public int MaxServerCapacity { get; set; }

        public int FPSTarget { get; set; }
        public string Protocol { get; set; }

        public string IPAddress { get; set; }

        public int Port { get; set; }

        public int ActiveConnCount { get; set; }

        public float RAM { get; set; }

        public float CPU { get; set; }
    }

}