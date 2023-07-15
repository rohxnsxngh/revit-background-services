using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Reflection;
using System.Diagnostics;
using revit_api_parse_model.migrations.data;
using revit_api_parse_model.models;


namespace revit_api_parse_model.migrations
{
    public class SQLDBConnect
    {
        private static string connectionString = "Data Source=SJC04MLTPRDDB.teslamotors.com;Initial Catalog=MfGfTx;Integrated Security=True";


        public static string TestSQLConn()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    List<string> tables = new List<string>(); //list of tables from sql
                    string queryString = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES";
                    SqlCommand command = new SqlCommand(queryString, connection);
                    connection.Open();

                    //read the data from sql query
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        //get name of column
                        if (!reader.HasRows)
                        {
                            //string results = "no data";
                            return "No Data.";
                        }
                        else
                        {
                            //Console.WriteLine(reader.GetName(0));
                            //move down each sql row
                            while (reader.Read())
                            {
                                tables.Add(reader.GetString(0));
                                //loop through column names and print value of row
                                /*for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    Console.Write(reader.GetValue(i));
                                    Console.WriteLine();
                                    return queryString;
                                }*/
                            }
                            return string.Join(",\n", tables);

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        public static string gettbldata()
        {
            return TblData.gettbldata();

        }

        public static int[]? getAppVersion()
        {
            return AppVersion.getAppVersion();
        }

        public static Dictionary<string, string>? getBuildingLevelsMap()
        {
            return BuildingLevelsMap.getBuildingLevelsMap();
        }

        public static Dictionary<string, string> getModelSchemaMap()
        {
            // remove null reference later
            return ModelSchemaMap.getModelSchemaMap()!;
        }

        public static Dictionary<string, int[]>? getTodaySyncCount()
        {
            return TodaySyncCount.getTodaySyncCount();
        }

        public static string postDocData(revit_document revDoc, string schema_name)
        {
            return PostDocData.postDocData(revDoc, schema_name);
        }
        public static string postDataSQL(fd_element? revElem, string schema_name)
        {
            return PostDataSQL.postDataSQL(revElem, schema_name);
        }
    }
}
