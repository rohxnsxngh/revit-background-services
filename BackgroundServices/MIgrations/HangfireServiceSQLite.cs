using System.Diagnostics;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Data.SQLite;
using Hangfire;
using Hangfire.Storage;
using Hangfire.Storage.SQLite;
using System.Collections.Concurrent;
using BackgroundServices.Models;
using BackgroundServices.Utility;
using System.Reflection;

namespace BackgroundServices.Migrations;

public class HangfireServiceSQLite
{
    public static void HangfireServiceSQLitePipeline()
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        string pathToSQLiteLocalDB = Path.Combine(currentDirectory, "BackgroundServiceApp.db");
        string sqliteConnectionString = $"Data Source={pathToSQLiteLocalDB};Version=3;";

        // PrintSQLiteSchema();
        // SQLiteTableValues();

        // StopAndDeleteHangfireServer();


    }

    public static void PrintSQLiteSchema()
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        string pathToSQLiteLocalDB = Path.Combine(currentDirectory, "BackgroundServiceApp.db");
        string sqliteConnectionString = $"Data Source={pathToSQLiteLocalDB};Version=3;";

        using (var connection = new SQLiteConnection(sqliteConnectionString))
        {
            connection.Open();

            DataTable schema = connection.GetSchema("Tables");

            foreach (DataRow row in schema.Rows)
            {
                string? tableName = row["TABLE_NAME"].ToString();
                Console.WriteLine($"Table: {tableName}");

                DataTable columns = connection.GetSchema("Columns", new[] { null, null, tableName });

                foreach (DataRow columnRow in columns.Rows)
                {
                    string? columnName = columnRow["COLUMN_NAME"].ToString();
                    Console.WriteLine($"- {columnName}");
                }

                Console.WriteLine();
            }
        }
    }

    public static void SQLiteTableValues()
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        string pathToSQLiteLocalDB = Path.Combine(currentDirectory, "BackgroundServiceApp.db");
        string sqliteConnectionString = $"Data Source={pathToSQLiteLocalDB};Version=3;";

        using (var connection = new SQLiteConnection(sqliteConnectionString))
        {
            connection.Open();

            using (var command = new SQLiteCommand("SELECT * FROM AggregatedCounter", connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    Console.WriteLine("Values in the JobParameter table:");

                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string columnName = reader.GetName(i);
                            object value = reader.GetValue(i);

                            Console.WriteLine($"{columnName}: {value}");
                        }

                        Console.WriteLine();
                    }
                }
            }
        }
    }

    // public static void ClearSQLiteDatabase()
    // {
    //     string currentDirectory = Directory.GetCurrentDirectory();
    //     string pathToSQLiteLocalDB = Path.Combine(currentDirectory, "BackgroundServiceApp.db");
    //     string sqliteConnectionString = $"Data Source={pathToSQLiteLocalDB};Version=3;";

    //     using (var connection = new SQLiteConnection(sqliteConnectionString))
    //     {
    //         connection.Open();

    //         // Retrieve the table names
    //         DataTable schema = connection.GetSchema("Tables");

    //         foreach (DataRow row in schema.Rows)
    //         {
    //             string tableName = row["TABLE_NAME"].ToString();

    //             // Delete all data from the table
    //             using (var command = new SQLiteCommand($"DELETE FROM {tableName}", connection))
    //             {
    //                 command.ExecuteNonQuery();
    //             }
    //         }
    //     }
    // }

    public static void StopAndDeleteHangfireServer()
    {
        // Stop the Hangfire server
        // var servers = JobStorage.Current.GetMonitoringApi().Servers();
        // if (servers.Any())
        // {
        //     BackgroundJobServer server = servers.First();
        //     server.SendStop();
        // }

        // Delete the SQLite database file
        string currentDirectory = Directory.GetCurrentDirectory();
        string pathToSQLiteLocalDB = Path.Combine(currentDirectory, "BackgroundServiceApp.db");
        if (File.Exists(pathToSQLiteLocalDB))
        {
            File.Delete(pathToSQLiteLocalDB);
        }

        // Restart the Hangfire server
        // var options = new SqliteStorageOptions
        // {
        //     AutoVacuumSelected = SQLiteStorageOptions.AutoVacuum.FULL,
        //     JobExpirationCheckInterval = TimeSpan.FromMinutes(10),
        //     InvisibilityTimeout = TimeSpan.FromHours(3),
        // };

        // GlobalConfiguration.Configuration.UseStorage(new SqliteStorage(pathToSQLiteLocalDB, options));
        // BackgroundJobServer newServer = new BackgroundJobServer();
    }

    // internal static bool DisposeServers()
    // {
    //     try
    //     {
    //         var type = Type.GetType("Hangfire.AppBuilderExtensions, Hangfire.Core", throwOnError: false);
    //         if (type == null) return false;

    //         var field = type.GetField("Servers", BindingFlags.Static | BindingFlags.NonPublic);
    //         if (field == null) return false;

    //         var value = field.GetValue(null) as ConcurrentBag<BackgroundJobServer>;
    //         if (value == null) return false;

    //         var servers = value.ToArray();

    //         foreach (var server in servers)
    //         {
    //             // Dispose method is a blocking one. It's better to send stop
    //             // signals first, to let them stop at once, instead of one by one.
    //             server.SendStop();
    //         }

    //         foreach (var server in servers)
    //         {
    //             server.Dispose();
    //         }

    //         return true;
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine("An exception occurred:");
    //         Console.WriteLine("Message: " + ex.Message);
    //         Console.WriteLine("Stack Trace: " + ex.StackTrace);
    //         Console.WriteLine("Inner Exception: " + ex.InnerException);
    //         return false;
    //     }
    // }

}