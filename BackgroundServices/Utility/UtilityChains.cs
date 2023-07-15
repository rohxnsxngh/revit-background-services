using System;
using System.Collections.Generic;
using System.Data;
using Newtonsoft.Json;
using Npgsql;

namespace BackgroundServices.Utility
{
    public static class UtilityChains
    {

        // private static string _subSystem = "UtilityChain";

        /// <summary>
        /// Select All Table Query String
        /// </summary>
        /// <param name="s">Table Name</param>
        /// <param name="where"></param>
        /// <returns></returns>
        // public static string GetTableSelectAll(this string s, string where = "")
        // {
        //     string m_s = $"SELECT * FROM {s} {where};";
        //     AppGlobalService.LogLine(
        //         _subSystem, 
        //         $"SQL: {m_s}", 
        //         LogType.Info);
        //     return m_s;
        // }

        /// <summary>
        /// DataTable to Native Element List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> ToItems<T>(this DataTable dt)
        {
            if (dt == null)
            {
                throw new ArgumentNullException(nameof(dt));
            }

            return JsonConvert.DeserializeObject<List<T>>(
                JsonConvert.SerializeObject(dt)
            )!;
        }

        /// <summary>
        /// Data Table to an Array of an Array of Values
        /// </summary>
        /// <param name="d"></param>
        /// <param name="includeHeaders"></param>
        /// <returns></returns>
        // internal static List<object[]> ToObjectArray(this DataTable d, bool includeHeaders = true)
        // {
        //     List<object[]> m_return = new List<object[]>();
        //     try
        //     {
        //         // Headers
        //         if (includeHeaders)
        //         {
        //             string[] m_header = new string[d.Columns.Count];
        //             for (int i = 0; i < d.Columns.Count; i++)
        //             {
        //                 m_header[i] = d.Columns[i].ColumnName;
        //             }

        //             m_return.Add(m_header);
        //         }

        //         // Rows
        //         foreach (DataRow x in d.Rows)
        //         {
        //             object[] m_row = new object[d.Columns.Count];
        //             for (int i = 0; i < d.Columns.Count; i++)
        //             {
        //                 m_row[i] = x[i];
        //             }
        //             m_return.Add(m_row);
        //         }

        //     }
        //     catch (Exception ex)
        //     {
        //         AppGlobalService.LogLine(
        //             _subSystem,
        //             "ToObjectArray",
        //             LogType.Error,
        //             ex);
        //     }
        //     return m_return;
        // }

        /// <summary>
        /// Command to DataTable
        /// </summary>
        /// <param name="sql">Full SQL Command</param>
        /// <param name="env">Environment Target</param>
        // /// <returns></returns>
        // internal static DataTable ToDataTable(this string sql, EnumDbEnv env)
        // {
        //     NpgsqlConnection m_conn = null;
        //     DataTable m_result = new DataTable();
        //     try
        //     {
        //         if (!sql.ToLower().StartsWith("select"))
        //         {
        //             return null;
        //         }

        //         // Select from DB
        //         m_conn = UtilityConnections.GetConn(env);
        //         m_conn.Open();
        //         using NpgsqlCommand cmd = new NpgsqlCommand(sql, m_conn);

        //         // Data set
        //         DataSet m_ds = new DataSet();
        //         NpgsqlDataAdapter m_adapter = new NpgsqlDataAdapter(cmd);
        //         m_adapter.Fill(m_ds);
        //         m_result = m_ds.Tables[0];
        //     }
        //     catch (Exception ex)
        //     {
        //         AppGlobalService.LogLine(
        //             _subSystem,
        //             $"ToDataTable: [{sql}]",
        //             LogType.Error,
        //             ex);
        //     }
        //     finally
        //     {
        //         m_conn?.Close();
        //     }
        //     return m_result;
        // }

        /// <summary>
        /// This function is FIRE!!!
        /// Get Native List of Elements from a Command
        /// </summary>
        /// <param name="cmd">Full Command</param>
        /// <returns></returns>
        // internal static List<T> ToItems<T>(this NpgsqlCommand cmd)
        // {
        //     List<T> m_return = new List<T>();
        //     try
        //     {
        //         // Target Table
        //         DataTable m_result = new DataTable();

        //         // Fill It
        //         DataSet m_ds = new DataSet();
        //         NpgsqlDataAdapter m_adapter = new NpgsqlDataAdapter(cmd);
        //         m_adapter.Fill(m_ds);

        //         // To a List of Items
        //         m_return = m_ds.Tables[0].ToItems<T>();

        //     }
        //     catch (Exception ex)
        //     {
        //         AppGlobalService.LogLine(
        //             _subSystem,
        //             $"ToItems: [{cmd.CommandText}]",
        //             LogType.Error,
        //             ex);
        //     }

        //     return m_return;
        // }

        /// <summary>
        /// Add JSON to a String
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Json(this string s)
        {
            return s.ToLower().EndsWith(".json")
                ? s
                : $"{s}.json";
        }

        /// <summary>
        /// No Null Strings
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string NoNull(this string s)
        {
            return s ?? "";
        }

        /// <summary>
        /// Simple Date
        /// </summary>
        /// <param name="d"></param>
        /// <param name="addMinutes"></param>
        /// <returns></returns>
        public static string ToSimpleString(this DateTime d, int addMinutes = 0)
        {
            // 2009-06-15T13:45:30
            DateTime m_date = d.AddMinutes(addMinutes);
            return
                $"TO_TIMESTAMP('{m_date.ToString()}', 'MM/DD/YYYY HH12:MI:SS')"; // $"{m_date.Year}-{m_date.Month:d2}-{m_date.Day:d2}T{m_date.Hour:d2}:{m_date.Minute:d2}";
        }

        /// <summary>
        /// Convert a string into 16-bit hash and returned as UUID (GUID)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Guid ToUuid(this string value)
        {
            return Guid.Parse(value);
        }

    }
}
