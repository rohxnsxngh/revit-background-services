using System;
using System.Reflection;

namespace BackgroundServices.Utility
{

    /// <summary>
    /// Domain Static Manager
    /// </summary>
    public class AppGlobalDomain
    {

        /// <summary>
        /// Domain Log Helper
        /// </summary>
        private static Logger? _log { get; set; }

        /// <summary>
        /// Log to File
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="t"></param>
        /// <param name="ex"></param>
        internal static void LogLine(string msg, LogType t, Exception? ex = null)
        {
            if (_log == null)
            {
                var version = Assembly.GetExecutingAssembly()?.GetName()?.Version?.ToString();
                _log = new Logger("RevitExportersDomain", version ?? "Unknown");
            }
            _log.LogLine(msg, t, ex);
        }

    }
}
