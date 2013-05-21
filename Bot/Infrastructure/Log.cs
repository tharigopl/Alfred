using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace Bot.Infrastructure
{
    public static class Log
    {
        private static readonly Lazy<Logger> logger = new Lazy<Logger>(() => LogManager.GetLogger("*"));
        private static Logger Logger { get { return logger.Value; } }

        public static void Debug(string format, params object[] parameters)
        {
            Logger.Debug(format, parameters);
        }

        public static void Info(string format, params object[] parameters)
        {
            Logger.Info(format, parameters);
        }

        public static void Warn(string format, params object[] parameters)
        {
            Logger.Warn(format, parameters);
        }

        public static void Error(string format, params object[] parameters)
        {
            Logger.Error(format, parameters);
        }

        public static void Fatal(string format, params object[] parameters)
        {
            Logger.Fatal(format, parameters);
        }
    }
}
