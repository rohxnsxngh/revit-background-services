using System;
using System.Data.SqlClient;
using Autodesk.Revit.UI;


namespace revit_api_parse_model.migrations.data;


public class AppVersion
{

    private static string connectionString = "Data Source=SJC04MLTPRDDB.teslamotors.com;Initial Catalog=MfGfTx;Integrated Security=True";
    public static int[]? getAppVersion()
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //List<string> tables = new List<string>(); //list of tables from sql
                string queryString = "SELECT * FROM revitapi.appversion";
                string version = "";
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                TaskDialog taskDialog = new TaskDialog("Appversion, Revit!");
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
                            version = reader.GetString(0);
                            taskDialog.MainContent = "Version: " + version;
                            taskDialog.Show();


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
                        string[] ver = version.Split('.');
                        int[] appversions = Array.ConvertAll(ver, s => int.Parse(s));
                        return appversions;

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
