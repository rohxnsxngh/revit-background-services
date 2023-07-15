using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using BackgroundServices.Utility;

namespace BackgroundServices.Utility
{
	public static class AppGlobalConsole
	{

		private static Logger? _log;
		internal static void LogLine(string msg, LogType l, Exception? ex = null)
		{
			if (_log == null)
			{
				_log = new Logger("ExportersConsole", GetAssemblyVersion());
			}

			string m_exception = "";
			if (ex != null)
			{
				m_exception = $" :: Exception Encountered: {ex.Message}";
			}
			System.Console.WriteLine($"{msg}{m_exception}");
			_log.LogLine(msg, l, ex);
		}

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
				LogLine("ZipAllFiles", LogType.Error, ex);
			}

			return null;
		}

	}
}
