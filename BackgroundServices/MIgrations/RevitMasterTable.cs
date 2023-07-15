using System.Diagnostics;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Concurrent;
using BackgroundServices.Models;
using BackgroundServices.Utility;

namespace BackgroundServices.Migrations;

public class RevitMasterTable
{
    private static string connectionString = "Data Source=SJC04MLTPRDDB.teslamotors.com;Initial Catalog=MfGfTx;Integrated Security=True;Connection Timeout=120;";
    private static string tableName = "revit_master";
    private static string BaseFolder = "RevitBatchProcessing";

    public static void RevitSyncMasterModels()
    {
        if (!TableExists(tableName))
        {
            Console.WriteLine($"Error: The table {tableName} does not exist.");
            return;
        }
        Console.WriteLine($"{tableName} Table Exists");

        RetrieveAllElementsFromTable();
    }

    public static void RetrieveAllElementsFromTable()
    {
        // SQL query to retrieve everything from a table
        string query = $"SELECT * FROM MfGfTx.gftxmfetl_dev.{tableName}";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    // Loop through the result set
                    while (reader.Read())
                    {
                        string revitYearVersion = reader.GetString(0);
                        string projectGUID = reader.GetString(1);
                        string modelGUID = reader.GetString(2);
                        string modelName = reader.GetString(3);

                        // string revitYearVersion = reader.GetString(reader.GetOrdinal("revit_year_version"));
                        // string projectGUID = reader.GetString(reader.GetOrdinal("project_guid"));
                        // string modelGUID = reader.GetString(reader.GetOrdinal("model_guid"));
                        // string modelName = reader.GetString(reader.GetOrdinal("model_name"));

                        string FileName = $"{modelName}.txt";
                        string ExpectedContent = $"{revitYearVersion} {projectGUID} {modelGUID}";

                        CheckFolderStructure(BaseFolder, modelName, FileName, ExpectedContent);
                        // Print the values to the console
                        Console.WriteLine($"{revitYearVersion} {projectGUID} {modelGUID} {modelName}");
                    }
                }
            }
        }
    }

    public static bool TableExists(string tableName)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            DataTable schema = connection.GetSchema("Tables");
            foreach (DataRow row in schema.Rows)
            {
                string? existingTableName = row[2] as string;
                if (string.Equals(existingTableName, tableName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }

    public static void CheckFolderStructure(string BaseFolder, string SubFolder, string FileName, string ExpectedContent)
    {

        string currentDirectory = Directory.GetCurrentDirectory();

        string repositoryRoot = RepositoryUtility.FindRepositoryRoot(currentDirectory);
        if (!string.IsNullOrEmpty(repositoryRoot))
        {
            Console.WriteLine("Repository path: " + repositoryRoot);
        }
        else
        {
            Console.WriteLine("No repository found.");
            return;
        }

        // Get the full path of the base folder
        string baseFolderPath = Path.Combine(repositoryRoot, BaseFolder);

        // Create the base folder if it doesn't exist
        if (!Directory.Exists(baseFolderPath))
        {
            Directory.CreateDirectory(baseFolderPath);
            Console.WriteLine($"Created folder: {baseFolderPath}");
        }

        // Get the full path of the subfolder
        string subFolderPath = Path.Combine(baseFolderPath, SubFolder);

        // Create the subfolder if it doesn't exist
        if (!Directory.Exists(subFolderPath))
        {
            Directory.CreateDirectory(subFolderPath);
            Console.WriteLine($"Created folder: {subFolderPath}");
        }

        // Get the full path of the file
        string filePath = Path.Combine(subFolderPath, FileName);

        // Create the file if it doesn't exist
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, ExpectedContent);
            Console.WriteLine($"Created file: {filePath}");
        }
        else
        {
            // File exists, check its content
            string fileContent = File.ReadAllText(filePath);

            // Compare the content with the expected value
            if (fileContent.Trim() == ExpectedContent)
            {
                Console.WriteLine($"File content is valid: {filePath}");
            }
            else
            {
                Console.WriteLine($"File content is invalid: {filePath}");
                // Replace the file content with new content
                File.WriteAllText(filePath, ExpectedContent);
                Console.WriteLine("File content replaced with new content.");
            }
        }
    }
}
