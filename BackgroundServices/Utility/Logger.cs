using Microsoft.Extensions.Logging;

namespace BackgroundServices.Utility;



/// <summary>
/// Log type
/// </summary>
public enum LogType
{

    /// <summary>
    /// Information
    /// </summary>
    Info,

    /// <summary>
    /// Error
    /// </summary>
    Error,

    /// <summary>
    /// Alert
    /// </summary>
    Alert
}

public class SimpleLogger
{
    private readonly ILogger _logger;

    public SimpleLogger(string v, ILogger<Logger> logger)
    {
        _logger = logger;
    }

    public void LogError(string message)
    {
        _logger.LogError(message);
    }

    // add more logging methods for different log levels if needed
}

/// <summary>
/// Logging Service
/// </summary>
public class Logger
{

    internal bool _serviceMode;

    private readonly string _safeAppName;
    private readonly string _ecpBuildId;
    private readonly char[] _invalidChars = Path.GetInvalidFileNameChars();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="appName"></param>
    /// <param name="build"></param>
    /// <param name="isService"></param>
    public Logger(string appName, string build, bool isService = false)
    {
        _safeAppName = GetSafeFileName(appName);
        _ecpBuildId = GetSafeFileName(build);
        _serviceMode = isService;
    }

    /// <summary>
    /// Log a Line to the Log
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="kind"></param>
    /// <param name="exc"></param>
    public void LogLine(string msg, LogType kind, Exception? exc = null)
    {
        try
        {
            if (string.IsNullOrEmpty(msg)) msg = "<NO MESSAGE>";
            string m_logLine = $"{DateTime.Now}\t{kind}\t{msg}";
            if (exc != null)
            {
                m_logLine += $"\t{exc}";
            }

            string m_path = GetFileName();
            if (!string.IsNullOrEmpty(m_path))
            {
                using (StreamWriter sw = new StreamWriter(m_path, true))
                {
                    sw.WriteLine(m_logLine);
                    sw.Close();
                }
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Logging error: {ex}");
        }
    }


    /// <summary>
    /// Safe Filename
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private string GetSafeFileName(string name)
    {
        string m_name = new string(name.Where(ch => !_invalidChars.Contains(ch)).ToArray());
        return string.IsNullOrEmpty(m_name)
            ? Guid.NewGuid().ToString()
            : m_name;
    }

    /// <summary>
    /// Filename
    /// </summary>
    /// <returns></returns>
    private string GetFileName()
    {
        try
        {
            string m_path = UtilityDomain.GetApplicationResourcePath("Logs", _serviceMode);
            if (!string.IsNullOrEmpty(m_path))
            {
                return Path.Combine(
                    m_path,
                    $"{_safeAppName}_{_ecpBuildId}_{DateTime.Now:yyy-MM-dd}.log");
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Get File Name error: {ex}");
        }
        return "";
    }

}
