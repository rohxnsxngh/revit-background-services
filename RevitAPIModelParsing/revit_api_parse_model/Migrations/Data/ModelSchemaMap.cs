using System;
using System.Text;
using Autodesk.Revit.UI;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using System.Diagnostics;
using revit_api_parse_model.models;
using System.Reflection;

namespace revit_api_parse_model.migrations
{
    public class ModelSchemaMap
    {
        private static string connectionString = "Data Source=SJC04MLTPRDDB.teslamotors.com;Initial Catalog=MfGfTx;Integrated Security=True";

        public static Dictionary<string, string>? getModelSchemaMap()
        {
            // try
            // {
            //     using (SqlConnection connection = new SqlConnection(connectionString))
            //     {
            //         List<string> tables = new List<string>(); //list of tables from sql
            //         string queryString = "SELECT * FROM gftxmfetl_dev.vwModelSchemaMap";

            //         SqlCommand command = new SqlCommand(queryString, connection);
            //         connection.Open();
            //         TaskDialog taskDialog = new TaskDialog("Building Floors, Revit!");
            //         Debug.WriteLine("reading building levels...");
            //         var model_schema_map = new Dictionary<string, string>();
            //         // read the data from sql query
            //         using (SqlDataReader reader = command.ExecuteReader())
            //         {
            //             // get name of column
            //             if (reader.HasRows)
            //             {
            //                 Console.WriteLine(reader.GetName(0));
            //                 // move down each sql row
            //                 while (reader.Read())
            //                 {
            //                     //*taskDialog.MainContent = reader.GetString(0);
            //                     taskDialog.Show();*//*
            //                     string model_name = (string)reader["model_name"];
            //                     string schema_name = (string)reader["schema_name"];

            //                     model_schema_map.Add(model_name, schema_name);
            //                     Debug.WriteLine("model name: " + model_name + " schema name: " + schema_name);
            //                     taskDialog.MainContent = "Workplane: " + work_plane + " Level Name: " + level_name;
            //                     taskDialog.Show();


            //                     *//*//loop through column names and print value of row
            //                     for (int i = 0; i < reader.FieldCount; i++)
            //                     {

            //                        *//* taskDialog.MainContent = reader.FieldCount.ToString();
            //                         taskDialog.Show();
            //                         Console.Write(reader.GetValue(i));
            //                         Console.WriteLine();*//*
            //                         tables.Add(reader.GetValue(i).ToString());

            //                     }*//*
            //                 }
            //                 *//*string[] ver = version.Split('.');
            //                 int[] appversions = Array.ConvertAll(ver, s => int.Parse(s));*//*
            //                 Debug.WriteLine("map" + model_schema_map.Count + model_schema_map.Values);
            //                 return model_schema_map;

            //             }
            //             else
            //             {
            //                 string results = "no data";
            //                 return model_schema_map;
            //             }

            //         }
            //     }
            // }
            // catch (Exception ex)
            // {
            //     Console.WriteLine(ex);
            //     return null;
            // }

            return null;
        }
    }
}