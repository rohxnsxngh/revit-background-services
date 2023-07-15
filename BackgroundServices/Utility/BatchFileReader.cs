using System;
using System.IO;
using System.Linq;

public class BatchFileReader
{
    public static int CheckLatestLogFile(string directoryPath, string fileNamePattern)
    {
        var logFiles = Directory.GetFiles(directoryPath, fileNamePattern)
                               .OrderByDescending(f => new FileInfo(f).CreationTimeUtc);

        var filePath = logFiles.FirstOrDefault(); // Get the first file path

        if (filePath != null)
        {
            Console.WriteLine(filePath);
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var streamReader = new StreamReader(fileStream))
            {
                while (!streamReader.EndOfStream)
                {
                    string? line = streamReader.ReadLine();

                    if (line != null && line.Contains("Task script operation completed"))
                    {
                        // Task Script Operation Completed
                        Console.WriteLine(line);
                        return 1;
                    }
                }
            }
        }

        // Task Script Operation was Incomplete
        Console.WriteLine("Log file not found or the required phrase was not found.");
        return 0;
    }

    public static string? GetLatestLogFile(string directoryPath, string fileNamePattern)
    {
        var logFiles = Directory.GetFiles(directoryPath, fileNamePattern)
                               .OrderByDescending(f => new FileInfo(f).CreationTimeUtc);

        return logFiles.FirstOrDefault();
    }
}