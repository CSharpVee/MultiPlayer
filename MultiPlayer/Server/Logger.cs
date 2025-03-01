﻿using MPModuleBase.Utilities;
using System.IO;
using System.Text;

namespace MultiPlayer.Server
{
    internal class LogInstance
    {
        public FileStream Stream;
        public StreamWriter StreamWriter;
        public StringBuilder StringBuild;

        public List<LoglineAdded> Callbacks = new List<LoglineAdded>();

        public string Filename { get; internal set; }
    }

    internal delegate void LoglineAdded();

    internal class Logger : ILogEngine
    {
        public static Logger Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Logger();

                return _instance;
            }
        }
        private static Logger _instance;

        private readonly string KLogFolder = "Logs";
        private IDictionary<int, LogInstance> _activeLogs;
        private int _nextLogId;

        private Logger()
        {
            _nextLogId = 1;
            Init();
        }

        ~Logger()
        {
            foreach (var entry in _activeLogs)
            {
                var inst = entry.Value;
                inst.StreamWriter.Flush();
                inst.StreamWriter.Dispose();
                inst.Stream.Dispose();
            }
        }

        private void Init()
        {
            _activeLogs = new Dictionary<int, LogInstance>();

            //Init main MP logging
            if (!Directory.Exists(KLogFolder))
                Directory.CreateDirectory(KLogFolder);

            var entry = OpenFile("MainLog.txt");
            _activeLogs.Add(0, entry);
        }

        public LogHandle StartLogging(string name)
        {
            var stamp = DateTime.Now;
            var path = $"{ name}_{ stamp.Year}{stamp.Month}{stamp.Day}{stamp.Hour}{stamp.Minute}__{_nextLogId}.txt";

            var entry = OpenFile(path);
            _activeLogs.Add(_nextLogId, entry);


            return new LogHandle(this, _nextLogId++);
        }

        public void LogMain(string msg)
        {
            DoWrite(_activeLogs[0], msg);
        }

        public void LogModule(int logID, string msg)
        {
            if (!_activeLogs.ContainsKey(logID))
                throw new ArgumentException("Trying to write to a nonexistant log");

            _activeLogs[logID].StreamWriter.Write(msg);

            DoWrite(_activeLogs[logID], msg);
        }

        public void CloseLog(int logID)
        {
            if (logID == 0)
                throw new InvalidOperationException("What are you doing?");

            var inst = _activeLogs[logID];
            inst.StreamWriter.Flush();
            inst.StreamWriter.Dispose();
            inst.Stream.Dispose();
        }

        public string GetLogtext(int logID)
        {
            return _activeLogs[logID].StringBuild.ToString();
        }

        public string GetLogFilename(int logID)
        {
            return _activeLogs[logID].Filename;
        }

        public void CallbackWhenChanged(int logID, LoglineAdded callback)
        {
            if (!_activeLogs.ContainsKey(logID))
                throw new InvalidOperationException("Trying to hook to a nonexistant log");

            _activeLogs[logID].Callbacks.Add(callback);
        }

        public void UnregisterCallback(int logID, LoglineAdded callback)
        {
            if (!_activeLogs.ContainsKey(logID) || !_activeLogs[logID].Callbacks.Contains(callback))
                throw new InvalidOperationException("Trying to remove a nonexistant entry");

            _activeLogs[logID].Callbacks.Remove(callback);
        }

        private void DoWrite(LogInstance inst, string logmsg)
        {
            var stamp = DateTime.Now;
            var txt = $"{stamp} : {logmsg}";
            inst.StreamWriter.WriteLine(txt);
            inst.StringBuild.AppendLine(txt);
            inst.StreamWriter.Flush();

            //inform change
            if (inst.Callbacks.Count > 0)
                foreach (var callback in inst.Callbacks)
                    callback();
        }

        private LogInstance OpenFile(string name)
        {
            var path = $@"{KLogFolder}\{name}";
            var fs = File.Open(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            var entry = new LogInstance()
            {
                Filename = path,
                Stream = fs,
                StreamWriter = new StreamWriter(fs),
                StringBuild = new StringBuilder()
            };

            return entry;
        }
    }
}
