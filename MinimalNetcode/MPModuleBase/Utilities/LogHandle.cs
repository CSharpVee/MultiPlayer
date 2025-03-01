﻿namespace MPModuleBase.Utilities
{
    public interface ILogEngine
    {
        public void LogModule(int logID, string msg);
        public string GetLogFilename(int logID);
    }

    public class LogHandle
    {
        private ILogEngine _logEngine;
        private int _logId;

        public LogHandle(ILogEngine logEngine, int inLogId)
        {
            _logEngine = logEngine;
            _logId = inLogId;
        }

        public void Log(string msg)
        {
            _logEngine.LogModule(_logId, msg);
        }

        public string GetFilename()
        {
            return _logEngine.GetLogFilename(_logId);
        }
    }
}
