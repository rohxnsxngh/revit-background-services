using System;
using System.Text;
using Autodesk.Revit.UI;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using System.Diagnostics;

namespace revit_api_parse_model.migrations
{
    public class TodaySyncCount
    {
        private static string connectionString = "Data Source=SJC04MLTPRDDB.teslamotors.com;Initial Catalog=MfGfTx;Integrated Security=True";
        public static Dictionary<string, int[]>? getTodaySyncCount()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    //List<string> tables = new List<string>(); //list of tables from sql
                    string queryString = "SELECT * FROM gftxmfetl_dev.vwTodaySyncCount";

                    SqlCommand command = new SqlCommand(queryString, connection);
                    connection.Open();
                    //TaskDialog taskDialog = new TaskDialog("Building Floors, Revit!");
                    Debug.WriteLine("reading sync counts...");
                    var today_sync_count = new Dictionary<string, int[]>();
                    //read the data from sql query
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        //get name of column
                        if (reader.HasRows)
                        {
                            //Console.WriteLine(reader.GetName(0));
                            //move down each sql row
                            while (reader.Read())
                            {
                                /*taskDialog.MainContent = reader.GetString(0);
                                taskDialog.Show();*/
                                string model_name = (string)reader["model_name"];
                                int sync_limit = (int)reader["sync_limit"];
                                int sync_count = (int)reader["sync_count"];

                                today_sync_count.Add(model_name, new int[] { sync_limit, sync_count });
                            }
                            /*string[] ver = version.Split('.');
                            int[] appversions = Array.ConvertAll(ver, s => int.Parse(s));*/
                            return today_sync_count;

                        }
                        else
                        {
                            //string results = "no data";
                            return today_sync_count;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
    }
}