using System.Collections.Generic;
using UnityEngine;

namespace com.perroelectrico.log {

    public enum LogLevel {
        Verbose,
        Debug,
        Info,
        Warning,
        Error,
        Fatal
    }

    public class Logger {

        private LogLevel minLevel = LogLevel.Debug;
        private string tag;
        private Logger(string tag) {
            this.tag = tag;
        }

        private static Dictionary<string, Logger> loggers;
        static Logger() {
            loggers = new Dictionary<string, Logger>();
        }

        private static object instanceLock = new object();

        public static Logger Get(string tag) {
            lock (instanceLock) {
                Logger logger;
                if (!loggers.TryGetValue(tag, out logger)) {
                    logger = new Logger(tag);
                    loggers[tag] = logger;
                }
                return logger;
            }
        }

        public void SetMinLevel(LogLevel level) {
            this.minLevel = level;
        }

        public void Log(LogLevel level, string message) {
            if (level < minLevel) {
                return;
            }
            switch (level) {
                case LogLevel.Verbose:
                    Debug.LogFormat("{0} VERBOSE: {1}", tag, message);
                    break;
                case LogLevel.Debug:
                    Debug.LogFormat("{0} DEBUG : {1}", tag, message);
                    break;
                case LogLevel.Info:
                    Debug.LogFormat("{0} INFO : {1}", tag, message);
                    break;
                case LogLevel.Warning:
                    Debug.LogWarningFormat("{0} WARN : {1}", tag, message);
                    break;
                case LogLevel.Error:
                    Debug.LogErrorFormat("{0} ERROR : {1}", tag, message);
                    break;
                case LogLevel.Fatal:
                    Debug.LogErrorFormat("{0} FATAL : {1}", tag, message);
                    break;
            }
        }

        public void Log(LogLevel level, string message, params object[] extra) {
            if (level < minLevel) {
                return;
            }
            Log(level, string.Format(message, extra));
        }

    }
}
