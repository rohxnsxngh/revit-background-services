using System;
using System.Text;
using Autodesk.Revit.UI;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using System.Diagnostics;

namespace revit_api_parse_model.migrations
{
    public class BuildingLevelsMap {

        private static string connectionString = "Data Source=SJC04MLTPRDDB.teslamotors.com;Initial Catalog=MfGfTx;Integrated Security=True";

         public static Dictionary<string, string>? getBuildingLevelsMap()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    //List<string> tables = new List<string>(); //list of tables from sql
                    string queryString = "SELECT * FROM revitapi.building_levels";

                    SqlCommand command = new SqlCommand(queryString, connection);
                    connection.Open();
                    //TaskDialog taskDialog = new TaskDialog("Building Floors, Revit!");
                    Debug.WriteLine("reading building levels...");
                    var building_levels_map = new Dictionary<string, string>();
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
                                string level_name = (string)reader["level_name"];
                                string level_name_standardized = (string)reader["level_name_standardized"];

                                building_levels_map.Add(level_name, level_name_standardized);
                                //taskDialog.MainContent = "Workplane: " + work_plane + " Level Name: " + level_name;
                                //taskDialog.Show();


                                /*//loop through column names and print value of row
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    
                                   *//* taskDialog.MainContent = reader.FieldCount.ToString();
                                    taskDialog.Show();
                                    Console.Write(reader.GetValue(i));
                                    Console.WriteLine();*//*
                                    tables.Add(reader.GetValue(i).ToString());
                                 
                                }*/
                            }
                            /*string[] ver = version.Split('.');
                            int[] appversions = Array.ConvertAll(ver, s => int.Parse(s));*/
                            return building_levels_map;

                        }
                        else
                        {
                            //string results = "no data";
                            return null;
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