using System.IO;
using System;

namespace BackgroundServices.Utility;
public static class RepositoryUtility
{
    // Find Base Directory
    public static string FindRepositoryRoot(string path)
    {
        DirectoryInfo? directory = new DirectoryInfo(path);

        while (directory != null && !DirectoryContainsGit(directory.FullName))
        {
            directory = directory.Parent;
        }

        return directory?.FullName ?? string.Empty;
    }

    // Use git to find Repo Base Path
    private static bool DirectoryContainsGit(string path)
    {
        string gitDirectoryPath = Path.Combine(path, ".git");
        return Directory.Exists(gitDirectoryPath);
    }

    // Find Specific File
    private static string? FindFile(List<string> folders, string fileName)
    {
        foreach (string folder in folders)
        {
            string filePath = Path.Combine(folder, fileName);
            if (File.Exists(filePath))
            {
                return filePath;
            }
        }

        return null;
    }
}
