using System;
using System.Globalization;
using System.IO;
using Assets.Scripts.Lib;

namespace Assets.Scripts {
    public class ScoreLogger : IDisposable {
        public string LogFile { get; private set; }
        private readonly StreamWriter _dataStream;
        private bool _disposed;

        public ScoreLogger(string logFile) {
            LogFile = logFile;
            var appName = Fnc.GetUnityApplicationName();
            var session = ((long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds).ToString(CultureInfo.InvariantCulture);
            var logFileInfo = !string.IsNullOrEmpty(LogFile) ? new FileInfo(LogFile.Replace("{app_name}", appName).Replace("{session}", session)) : null;

            if (logFileInfo == null || logFileInfo.Directory == null) {
                throw new Exception("Invalid log file specified!");
            }

            if (!logFileInfo.Directory.Exists) logFileInfo.Directory.Create();
            _dataStream = new StreamWriter(new FileStream(logFileInfo.FullName, FileMode.Append, FileAccess.Write));
            const string header = "Timestamp,PlayerId,Round,Round Score,Total Score";
            _dataStream.WriteLine(header);
        }

        public void Log(uint userId, int round, int roundScore, int totalScore) {
            var timestamp = DateTime.Now.ToString("O");
            var dataline = timestamp + "," + userId + "," + round + "," + roundScore + "," + totalScore;
            _dataStream.WriteLine(dataline);
        }

        public void Dispose() {
            if (_disposed) return;
            _dataStream.Flush();
            _dataStream.Close();
            _disposed = true;
        }
    }
}
