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
using BackgroundServices.Migrations;
using BackgroundServices.Utility;
using Hangfire;
using System.Threading;
using System.Threading.Tasks;
using BackgroundServices.Models;

namespace BackgroundServices.Services;

public class ServiceManagement : IServiceManagement
{

    [AutomaticRetry(Attempts = 0)]
    public async Task<SyncResult> SyncIndividualModelsToDatabase(List<string> revitModelNames, CancellationToken cancellationToken)
    {
        try
        {
            bool hasZeroLogStatus = false;
            var modelsWithZeroLogStatus = new List<string>();

            // Check if cancellation has been requested
            cancellationToken.ThrowIfCancellationRequested();
            Console.WriteLine($"Sync Database: Task Created {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");

            string currentDirectory = Directory.GetCurrentDirectory();
            string repoPath = RepositoryUtility.FindRepositoryRoot(currentDirectory);
            string RevitBatchProcessing = "RevitBatchProcessing";
            string rootFolders = Path.Combine(repoPath, RevitBatchProcessing);

            var tasks = new List<Task>();

            SemaphoreSlim semaphore = new SemaphoreSlim(5); // Set the maximum concurrent tasks (this is sick!!!!)

            foreach (string modelName in revitModelNames)
            {
                await semaphore.WaitAsync(); // Wait until a slot is available
                Task task = Task.Run(async () =>
                {
                    try
                    {
                        Process process = new Process();

                        string? filePath = Path.Combine(rootFolders, "SyncDatabase.bat");

                        if (!string.IsNullOrEmpty(filePath))
                        {
                            Console.WriteLine("File found at: " + filePath);

                            process.StartInfo.FileName = "cmd.exe";
                            process.StartInfo.Arguments = "/c " + filePath + " " + modelName;
                            process.StartInfo.UseShellExecute = false;
                            process.StartInfo.RedirectStandardOutput = true;
                            process.StartInfo.CreateNoWindow = true;

                            process.Start();
                            string output = await process.StandardOutput.ReadToEndAsync();
                            Console.WriteLine(output);

                            process.WaitForExit();
                            int exitCode = process.ExitCode;

                            string individualModelPath = Path.Combine(rootFolders, modelName);
                            Console.WriteLine(individualModelPath);
                            int logStatus = BatchFileReader.CheckLatestLogFile($"{individualModelPath}/loggingBatchProcessor", "*.log");
                            Console.WriteLine("Log Status:" + logStatus + " " + modelName);

                            if (logStatus == 0)
                            {
                                modelsWithZeroLogStatus.Add(modelName);
                                hasZeroLogStatus = true;
                                string? fileName = BatchFileReader.GetLatestLogFile($"{individualModelPath}\\loggingBatchProcessor", "*.log");
                                Stream fileStream = AWSS3MigrationService.GetFileStream(fileName!);

                                // Create an instance of AWSS3MigrationService
                                AWSS3MigrationService s3MigrationService = new AWSS3MigrationService();

                                // Call the UploadObjectToS3Bucket method using the instance
                                await s3MigrationService.UploadObjectToS3Bucket(fileName!, fileStream, modelName);

                            }

                            Console.WriteLine($"Sync Database for model '{modelName}': Task Completed {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
                        }
                        else
                        {
                            Console.WriteLine("SyncDatabase.bat not found in the specified root folder.");
                        }
                    }
                    finally
                    {
                        semaphore.Release(); // Release the slot
                    }
                });

                tasks.Add(task);
            }

            await Task.WhenAll(tasks);

            List<string> failedModelNames = modelsWithZeroLogStatus.ToList();

            if (hasZeroLogStatus)
            {
                return new SyncResult
                {
                    Result = 0,
                    FailedModelNames = failedModelNames
                };
            }

            return new SyncResult
            {
                Result = 1,
                FailedModelNames = failedModelNames
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return new SyncResult
            {
                Result = 0,
                FailedModelNames = new List<string>()
            };
        }
    }


    [AutomaticRetry(Attempts = 0)]
    public void SyncRevitMasterFilesWithRevitBatchProcesssingStructure(CancellationToken cancellationToken)
    {
        // TestSQLConnection.TestSQLConnectionMethod();
        // Check if cancellation has been requested
        cancellationToken.ThrowIfCancellationRequested();
        RevitMasterTable.RevitSyncMasterModels();
        Console.WriteLine($"Sync Master Revit Files: Long running task {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
    }

    [AutomaticRetry(Attempts = 0)]
    public void UpdateDatabase(CancellationToken cancellationToken)
    {
        // Check if cancellation has been requested
        cancellationToken.ThrowIfCancellationRequested();
        Console.WriteLine($"Update Database: Short running task {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
    }

    [AutomaticRetry(Attempts = 0)]
    public void DeleteLocalLoggingFile(CancellationToken cancellationToken)
    {
        // Check if cancellation has been requested
        cancellationToken.ThrowIfCancellationRequested();
        Console.WriteLine($"Delete LoggingFiles: Short running task {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
        string currentDirectory = Directory.GetCurrentDirectory();
        string repoPath = RepositoryUtility.FindRepositoryRoot(currentDirectory);
        string RevitBatchProcessing = "RevitBatchProcessing";
        string rootFolders = Path.Combine(repoPath, RevitBatchProcessing);

        try
        {
            // Search for loggingBatchProcessor directories recursively
            string[] loggingDirectories = Directory.GetDirectories(rootFolders, "loggingBatchProcessor*", SearchOption.AllDirectories);

            foreach (string loggingDirectory in loggingDirectories)
            {
                // Delete .log files
                string[] logFiles = Directory.GetFiles(loggingDirectory, "*.log");
                foreach (string logFile in logFiles)
                {
                    File.Delete(logFile);
                    Console.WriteLine($"Deleted log file: {logFile}");
                }

                // Delete .txt files
                string[] textFiles = Directory.GetFiles(loggingDirectory, "*.txt");
                foreach (string textFile in textFiles)
                {
                    File.Delete(textFile);
                    Console.WriteLine($"Deleted text file: {textFile}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    [AutomaticRetry(Attempts = 0)]
    public async Task ServiceDatabase(List<string> revitModelNames, string? userName, CancellationToken cancellationToken)
    {
        // Check if cancellation has been requested
        cancellationToken.ThrowIfCancellationRequested();
        var dateCreated = DateTime.Now;
        SyncResult syncResult = await this.SyncIndividualModelsToDatabase(revitModelNames, cancellationToken);
        int logStatus = syncResult.Result;
        List<string> failedModels = syncResult.FailedModelNames;
        var dateCompleted = DateTime.Now;
        ServicesControlTable.ServicesInsertControlTableMethod(revitModelNames, userName, dateCompleted, dateCreated, logStatus, failedModels);
    }

    public void HangfireServiceSQLiteTracking(CancellationToken cancellationToken)
    {
        // Check if cancellation has been requested
        cancellationToken.ThrowIfCancellationRequested();
        Console.WriteLine("Hangfire SQLite Service Tracking");
        HangfireServiceSQLite.HangfireServiceSQLitePipeline();
    }
}

