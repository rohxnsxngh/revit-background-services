using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BackgroundServices.Models;

namespace BackgroundServices.Migrations;

public class ServicesControlTable
{
    private static string connectionString = "Data Source=SJC04MLTPRDDB.teslamotors.com;Initial Catalog=MfGfTx;Integrated Security=True;Connection Timeout=120;";

    public static void ServicesInsertControlTableMethod(List<string> modelNames, string? userName, DateTime? dateCompleted, DateTime? dateCreated, int logStatus, List<string> failedModels)
    {
        SyncModel model = new SyncModel
        {
            job_id = Guid.NewGuid(),
            user_name = userName ?? "scheduled task",
            model_name = modelNames,
            date_created = dateCreated,
            date_completed = dateCompleted,
            status = logStatus,
            failed_model = failedModels
        };

        InsertSyncModel(model);
    }

    public static void InsertSyncModel(SyncModel model)
    {
        if (!TableExists("services_control_table"))
        {
            Console.WriteLine("Error: The table 'services_control_table' does not exist.");
            return;
        }

        if (string.IsNullOrEmpty(model.user_name))
        {
            model.user_name = "scheduled task";
        }

        if (model.user_name != "rohasingh" && model.user_name != "csasam" && model.user_name != "flam" && model.user_name != "mkhaled" && model.user_name != "grbell" && model.user_name != "scheduled task")
        {
            throw new UnauthorizedAccessException();
        }

        try
        {

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO MfGfTx.gftxmfetl_dev.services_control_table (job_id, user_name, model_name, date_created, date_completed, status, failed_model) " +
                               "VALUES (@jobId, @userName, @modelName, @dateCreated, @dateCompleted, @status, @failedModel)";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@jobId", model.job_id);
                command.Parameters.AddWithValue("@userName", model.user_name);
                command.Parameters.AddWithValue("@modelName", string.Join(", ", model.model_name));
                command.Parameters.AddWithValue("@dateCreated", model.date_created);
                command.Parameters.AddWithValue("@dateCompleted", model.date_completed);
                command.Parameters.AddWithValue("@status", model.status);
                command.Parameters.AddWithValue("@failedModel", string.Join(", ", model.failed_model));

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                Console.WriteLine($"Rows inserted: {rowsAffected}");
            }
        }
        catch (SqlException ex)
        {
            Console.WriteLine("Error: " + ex.Message);
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
}
