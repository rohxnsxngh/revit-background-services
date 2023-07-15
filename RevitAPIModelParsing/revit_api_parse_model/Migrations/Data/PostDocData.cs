using System;
using System.Text;
using Autodesk.Revit.UI;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using System.Diagnostics;
using revit_api_parse_model.models;

namespace revit_api_parse_model.migrations
{
    public class PostDocData
    {
        private static string connectionString = "Data Source=SJC04MLTPRDDB.teslamotors.com;Initial Catalog=MfGfTx;Integrated Security=True";
        public static string postDocData(revit_document revDoc, string schema_name)
        {
            var sb = new StringBuilder();
            try
            {
                TaskDialog taskDialog = new TaskDialog("SQL Document Insert");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand((schema_name + ".Insert_DocProps"), conn);
                    cmd.Parameters.Clear();
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.Add(new SqlParameter("@guid_project", revDoc.guid_project));
                    cmd.Parameters.Add(new SqlParameter("@doc_id", revDoc.doc_id));
                    cmd.Parameters.Add(new SqlParameter("@model_name", revDoc.model_name));
                    cmd.Parameters.Add(new SqlParameter("@model_desc", revDoc.model_desc));
                    cmd.Parameters.Add(new SqlParameter("@model_path", revDoc.model_path));
                    cmd.Parameters.Add(new SqlParameter("@sync_time", revDoc.sync_time));
                    cmd.Parameters.Add(new SqlParameter("@user_name", revDoc.user_name));
                    cmd.Parameters.Add(new SqlParameter("@user_machine", revDoc.user_machine));
                    cmd.Parameters.Add(new SqlParameter("@sync_id", revDoc.sync_id));
                    cmd.ExecuteNonQuery();
                }
                return "Doc table updated";
            }
            catch (Exception ex)
            {
                sb.Append("Document Insert Failed").ToString();
                sb.AppendLine(ex.Message);
                return sb.ToString();
            }
        }
    }
}