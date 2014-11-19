using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Assets.Scripts.Kinect.Gesture;
using Assets.Scripts.Lib;
using UnityEngine;

namespace Assets.Scripts.Kinect {
    public class JointLogger : IDisposable {
        public string LogFile { get; private set; }
        private readonly StreamWriter _dataStream;
        private bool _disposed;

        public JointLogger(string logFile) {
            LogFile = logFile;
            var appName = Fnc.GetUnityApplicationName();
            var session = ((long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds).ToString(CultureInfo.InvariantCulture);
            var logFileInfo = !string.IsNullOrEmpty(LogFile) ? new FileInfo(LogFile.Replace("{app_name}", appName).Replace("{session}", session)) : null;

            if (logFileInfo == null || logFileInfo.Directory == null) {
                throw new Exception("Invalid log file specified!");
            }

            if (!logFileInfo.Directory.Exists) logFileInfo.Directory.Create();
            _dataStream = new StreamWriter(new FileStream(logFileInfo.FullName, FileMode.Append, FileAccess.Write));
            var header = "Timestamp,PlayerId," + string.Join(",", Enum.GetNames(typeof(Joints)).Select(joint => joint + ".x," + joint + ".y," + joint + ".z").ToArray()) + ",Gesture";
            _dataStream.WriteLine(header);
        }

        public void Log(JointPositions positions) {
            Log(positions, 0);
        }

        public void Log(JointPositions positions, uint userId) {
            Log(positions, userId, new IGesture[] {null});
        }

        public void Log(JointPositions positions, uint userId, IGesture gesture) {
            Log(positions, userId, new[]{gesture});
        }

        public void Log(JointPositions positions, uint userId, IList<IGesture> gestures) {
            positions.InitIfNotSetExt(Fnc.Values<Joints>());

            var timestamp = DateTime.Now.ToString("O");
            var player = userId.ToString(CultureInfo.InvariantCulture);
            var positionsDump = string.Join(",", Fnc.Values<Joints>().Select(joint => positions[joint].x + "," + positions[joint].y + "," + positions[joint].z).ToArray());
            var baseDataLine = timestamp + "," + player + "," + positionsDump + ",";
            if (gestures == null || gestures.Count < 1) gestures = new IGesture[] {null};

            foreach (var dataLine in gestures.Select(gestureDetected => baseDataLine + (gestureDetected != null ? gestureDetected.Gesture.ToString() : "None"))) {
                _dataStream.WriteLine(dataLine);
            }
        }

        public void Dispose() {
            if (_disposed) return;
            _dataStream.Flush();
            _dataStream.Close();
            _disposed = true;
        }
    }
}
