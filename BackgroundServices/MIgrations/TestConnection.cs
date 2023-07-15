using System;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace BackgroundServices.Migrations;

public class TestSQLConnection
{
    private static string connectionString = "Data Source=SJC04MLTPRDDB.teslamotors.com;Initial Catalog=MfGfTx;Integrated Security=True";

    public static void TestSQLConnectionMethod()
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                Console.WriteLine("Connection opened successfully.");

                // Perform database operations here

                connection.Close();
                Console.WriteLine("Connection closed successfully.");
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}