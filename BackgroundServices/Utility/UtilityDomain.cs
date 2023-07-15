using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using BackgroundServices.Utility;

namespace BackgroundServices.Utility
{
    public static class UtilityDomain
    {

        /// <summary>
        /// Assembly Version
        /// </summary>
        /// <returns></returns>
        public static string GetAssemblyVersion()
        {
            var assemblyName = Assembly.GetExecutingAssembly().GetName();
            return assemblyName?.Version?.ToString() ?? "Unknown";
        }

        /// <summary>
        /// Path to File used to store values for NWC Automation Requests
        /// </summary>
        /// <returns></returns>
        public static string GetAutomationRequestFilePath()
        {
            string m_return = "";
            try
            {
                string m_path = GetApplicationResourcePath("Automation");
                if (Directory.Exists(m_path))
                {
                    m_return = Path.Combine(m_path, "revit-automation-request-nwc.json");
                }
            }
            catch (Exception ex)
            {
                AppGlobalDomain.LogLine(
                    "GetAutomationRequestFilePath",
                    LogType.Error,
                    ex);
            }
            return m_return;
        }

        /// <summary>
        /// ECP Files under the user's profile
        /// </summary>
        /// <param name="subDirName"></param>
        /// <param name="serviceLocation"></param>
        /// <returns></returns>
        public static string GetApplicationResourcePath(string subDirName, bool serviceLocation = false)
        {
            try
            {
                string m_sd = $@"Tesla\GigaTX\{subDirName}";
                string m_prefix =
                    Path.Combine(
                        Environment.GetFolderPath(
                            Environment.SpecialFolder.LocalApplicationData),
                        m_sd);

                if (serviceLocation)
                {
                    m_prefix =
                        Path.Combine(
                            Environment.GetFolderPath(
                                Environment.SpecialFolder.CommonApplicationData),
                            m_sd);
                }

                if (!Directory.Exists(m_prefix))
                {
                    Directory.CreateDirectory(m_prefix);
                }

                if (Directory.Exists(m_prefix)) return m_prefix;
            }
            catch (Exception ex)
            {
                AppGlobalDomain.LogLine(
                    "GetApplicationResourcePath",
                    LogType.Error,
                    ex);
            }
            return "";
        }

        /// <summary>
        /// Get the raw string
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal static string GetRawString(string name)
        {
            string m_return = "";
            try
            {
                Stream? m_s = GetResourceStream(name);
                if (m_s != null)
                {
                    using (StreamReader s = new StreamReader(m_s))
                    {
                        m_return = s.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                AppGlobalDomain.LogLine(
                    "GetRawString",
                    LogType.Error,
                    ex);
            }
            return m_return;
        }

        /// <summary>
        /// Get Resource Stream
        /// </summary>
        /// <param name="resourcePath"></param>
        /// <returns></returns>
        internal static Stream? GetResourceStream(string resourcePath)
        {
            try
            {
                Assembly m_a = Assembly.GetExecutingAssembly();
                List<string> m_rsc = new List<string>(m_a.GetManifestResourceNames());

                resourcePath = resourcePath.Replace(@"/", ".");
                string? foundResourcePath = m_rsc.FirstOrDefault(r => r.Contains(resourcePath));

                return foundResourcePath == null
                    ? null
                    : m_a.GetManifestResourceStream(foundResourcePath);
            }
            catch (Exception ex)
            {
                AppGlobalDomain.LogLine(
                    "GetResourceStream",
                    LogType.Error,
                    ex);
            }

            return null;
        }


        /// <summary>
        /// Safe File Naming Characters
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetSafeFileName(string name)
        {
            string m_chars = new string(Path.GetInvalidFileNameChars());
            string m_escape = Regex.Escape(m_chars);
            string m_reg = string.Format(@"([{0}]*\.+$)|([{0}]+)", m_escape);
            return Regex.Replace(name, m_reg, "");
        }

        /// <summary>
        /// Checks to see if at least 1GB is free on C drive
        /// </summary>
        /// <param name="mbThreshold"></param>
        /// <returns></returns>
        public static bool IsHdSpaceAvailable(int mbThreshold)
        {
            foreach (DriveInfo x in DriveInfo.GetDrives())
            {
                if (x.IsReady)
                {
                    if (x.Name.ToLower().StartsWith("c"))
                    {
                        return (ConvertBytesToMegabytes(x.AvailableFreeSpace) > mbThreshold);
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Bytes to Megabytes
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static double ConvertBytesToMegabytes(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }


        #region Public Members - IO UNC Paths

        [DllImport("mpr.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int WNetGetConnection(
            [MarshalAs(UnmanagedType.LPTStr)] string localName,
            [MarshalAs(UnmanagedType.LPTStr)] StringBuilder remoteName,
            ref int length);

        /// <summary>
        /// Given a path, returns the UNC path or the original. (No exceptions are raised by this function directly)
        /// For example, "P:\2008-02-29" might return: "\\networkserver\Shares\Photos\2008-02-09"
        /// </summary>
        /// <param name="originalPath">The path to convert to a UNC Path</param>
        /// <returns>A UNC path. If a network drive letter is specified, the drive letter is converted to a UNC or network path. If the 
        /// originalPath cannot be converted, it is returned unchanged.</returns>
        public static string GetUncPath(string originalPath)
        {
            try
            {
                StringBuilder sb = new StringBuilder(512);
                int size = sb.Capacity;

                // Look for the {LETTER}: combination ...
                if (originalPath.Length > 2 && originalPath[1] == ':')
                {
                    // Don't use char.IsLetter here - as that can be misleading.
                    // The only valid drive letters are a-z && A-Z.
                    char c = originalPath[0];

                    if (c.ToString().ToLower() == "c")
                    {
                        return $@"\\{Environment.MachineName}\c$\{originalPath.Substring(3)}";
                    }

                    if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
                    {
                        int m_error = WNetGetConnection(originalPath.Substring(0, 2), sb, ref size);
                        if (m_error == 0)
                        {
                            DirectoryInfo m_dir = new DirectoryInfo(originalPath);

                            string? pathRoot = Path.GetPathRoot(originalPath);
                            if (pathRoot != null)
                            {
                                string m_path = Path.GetFullPath(originalPath).Substring(pathRoot.Length);
                                return Path.Combine(sb.ToString().TrimEnd(), m_path);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AppGlobalDomain.LogLine(
                    "GetUncPath",
                    LogType.Error,
                    ex);
            }
            return originalPath;
        }

        #endregion

        #region Public Members - IO Compression

        /// <summary>
        /// Compress all files in a directory to GZ
        /// </summary>
        /// <param name="di">Directory to process</param>
        /// <param name="searchPattern">Can be used to filter by a file extension or name part</param>
        /// <param name="includeNestedDirectories">Will include all files in nested directories if set to true</param>
        /// <param name="deleteOriginalOnSuccess">Optionally delete the original file if compressed successfully</param>
        /// <returns></returns>
        // public static ErrorResponse SaveAllToGz(this DirectoryInfo di,
        // 	 string searchPattern,
        // 	 bool includeNestedDirectories,
        // 	 bool deleteOriginalOnSuccess = false)
        // {
        // 	try
        // 	{

        // 		if (!Directory.Exists(di.FullName))
        // 			return new ErrorResponse(LogType.Error, $"Directory Not Found: {di.FullName}");

        // 		if (string.IsNullOrEmpty(searchPattern))
        // 			searchPattern = "*";

        // 		SearchOption m_opt = includeNestedDirectories
        // 			 ? SearchOption.AllDirectories
        // 			 : SearchOption.TopDirectoryOnly;

        // 		foreach (FileInfo x in di.GetFiles(searchPattern, m_opt))
        // 		{
        // 			if (x.DirectoryName != null)
        // 				x.SaveToGz($"{x.FullName}.gz",
        // 					 deleteOriginalOnSuccess);
        // 		}

        // 		return null;

        // 	}
        // 	catch (Exception ex)
        // 	{
        // 		AppGlobalDomain.LogLine(
        // 			"SaveAllToGz",
        // 			LogType.Error,
        // 			ex);
        // 		return new ErrorResponse(LogType.Error, "CompressAllFilesToGz Encountered an Exception", ex);
        // 	}
        // }

        /// <summary>
        /// Compress a file to GZ
        /// </summary>
        /// <param name="file">File to be compressed</param>
        /// <param name="fileOutPath">Output file</param>
        /// <param name="deleteOriginalOnSuccess">Optionally remove the original file if successfully compressed as output</param>
        // public static ErrorResponse SaveToGz(this FileInfo file,
        // 	 string fileOutPath,
        // 	 bool deleteOriginalOnSuccess = false)
        // {
        // 	try
        // 	{

        // 		if (!File.Exists(file.FullName))
        // 			return new ErrorResponse(LogType.Error, $"File Not Found: {file.FullName}");

        // 		if (File.Exists(fileOutPath))
        // 			File.Delete(fileOutPath);

        // 		byte[] b;
        // 		using (FileStream f = new FileStream(file.FullName, FileMode.Open))
        // 		{
        // 			b = new byte[f.Length];
        // 			f.Read(b, 0, (int)f.Length);
        // 		}

        // 		// GZipStream to file
        // 		using (FileStream f2 = new FileStream(fileOutPath, FileMode.Create))
        // 		using (GZipStream gz = new GZipStream(f2, CompressionMode.Compress, false))
        // 		{
        // 			gz.Write(b, 0, b.Length);
        // 		}

        // 		if (!File.Exists(fileOutPath))
        // 			return new ErrorResponse(LogType.Error, "Failed to Validate File After Write");

        // 		if (deleteOriginalOnSuccess) File.Delete(file.FullName);
        // 		return null;

        // 	}
        // 	catch (Exception ex)
        // 	{
        // 		AppGlobalDomain.LogLine(
        // 			"SaveToGz",
        // 			LogType.Error,
        // 			ex);
        // 		return new ErrorResponse(LogType.Error, "CompressFileToGz Encountered an Exception", ex);
        // 	}
        // }

        /// <summary>
        /// Zip a list of Files into a single zip file
        /// </summary>
        /// <param name="saveFolder">Location to save the zip file</param>
        /// <param name="filesToZip">Files to be zipped</param>
        /// <param name="zipFilename">Name for zip file</param>
        /// <returns>Reference to resulting zip file</returns>
        public static FileInfo? ZipAllFiles(DirectoryInfo saveFolder, List<FileInfo> filesToZip, string zipFilename)
        {
            if (!Directory.Exists(saveFolder.FullName)) return null;
            FileInfo m_filePath = new FileInfo(Path.Combine(saveFolder.FullName, $"{zipFilename}.zip"));
            try
            {
                if (m_filePath.Exists) m_filePath.Delete();
                using (MemoryStream ms = new MemoryStream())
                {
                    using (ZipArchive z = new ZipArchive(ms, ZipArchiveMode.Create, true))
                    {
                        foreach (FileInfo x in filesToZip)
                        {
                            ZipArchiveEntry m_file = z.CreateEntry(x.Name);
                            using (Stream s = m_file.Open())
                            using (BinaryWriter f = new BinaryWriter(s))
                            {
                                byte[] m_contents = File.ReadAllBytes(x.FullName);
                                f.Write(m_contents);
                            }
                        }
                    }

                    using (FileStream fs = new FileStream(m_filePath.FullName, FileMode.Create))
                    {
                        ms.Seek(0, SeekOrigin.Begin);
                        ms.CopyTo(fs);
                    }
                }

                return m_filePath;
            }
            catch (Exception ex)
            {
                AppGlobalDomain.LogLine(
                    "ZipAllFiles",
                    LogType.Error,
                    ex);
            }
            return null;
        }

        #endregion

    }
}
