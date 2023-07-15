using System;
using System.Text;
using Autodesk.Revit.UI;
using System.Data.SqlClient;
using System.Data;

namespace revit_api_parse_model.migrations.data;

public class TblData
{

    private static string connectionString = "Data Source=SJC04MLTPRDDB.teslamotors.com;Initial Catalog=MfGfTx;Integrated Security=True";

    public static string gettbldata()
    {
        var sb = new StringBuilder();
        try
        {
            TaskDialog taskDialog = new TaskDialog("---Check Stored Procedure---");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("revitapi_nova.getDesignOptions", conn);
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                cmd.Parameters.Add(new SqlParameter("@model_name", "BASIC FACTORY_TEST FILE"));
                cmd.ExecuteNonQuery();
                using (var reader = cmd.ExecuteReader())
                {

                    sb.AppendLine(reader.Read().ToString());
                }

                taskDialog.MainContent = sb.ToString();
                taskDialog.Show();
            }
            return "Get table data completed";
        }
        catch (Exception ex)
        {
            sb.Append("Failed").ToString();
            sb.AppendLine(ex.Message);
            return sb.ToString();
        }

    }
}
