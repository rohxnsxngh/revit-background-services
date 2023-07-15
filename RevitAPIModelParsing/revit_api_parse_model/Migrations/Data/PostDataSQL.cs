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
    public class PostDataSQL
    {
        private static string connectionString = "Data Source=SJC04MLTPRDDB.teslamotors.com;Initial Catalog=MfGfTx;Integrated Security=True";
        public static string postDataSQL(fd_element? revElem, string schema_name)
        {
            TaskDialog taskDialog = new TaskDialog("SQLDB");
            var sb = new StringBuilder();

            sb.Append("PC_Name:" + Environment.MachineName.ToString() + "\n");
            sb.Append("User_Name:" + Environment.UserName.ToString() + "\n");
            //sb.Append("GUID: " + sync_id + "\n");

            //fd_element revElems = new fd_element();
            //make sure to account for new data class types like wip
            //var propType = revMass != null ? revMass.GetType().GetProperties() : revAisle.GetType().GetProperties();
            PropertyInfo[] propType = new PropertyInfo[0]; // Updated initialization
            // PropertyInfo[] propType = null;

            /*taskDialog.MainContent = sb.ToString();
            taskDialog.Show();
            return "complete";*/
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand((schema_name + ".Insert_RevitElements"), connection);
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection.Open(); // connection.Open();
                //cmd.Parameters.Add(new SqlParameter("@user_name", Environment.UserName.ToString()));
                //cmd.Parameters.Add(new SqlParameter("@pc_name", Environment.MachineName.ToString()));
                if (revElem != null)
                {
                    propType = revElem.GetType().GetProperties();
                    cmd.Parameters.Add(new SqlParameter("@element_uid", revElem.element_uid));
                    cmd.Parameters.Add(new SqlParameter("@element_id", revElem.element_id));
                    cmd.Parameters.Add(new SqlParameter("@sync_id", revElem.sync_id));
                    cmd.Parameters.Add(new SqlParameter("@design_option_set", revElem.design_option_set));
                    cmd.Parameters.Add(new SqlParameter("@design_option_name", revElem.design_option_name));
                    cmd.Parameters.Add(new SqlParameter("@design_option_is_primary", revElem.design_option_is_primary));
                    cmd.Parameters.Add(new SqlParameter("@category_name", revElem.category_name));
                    cmd.Parameters.Add(new SqlParameter("@workset_name", revElem.workset_name));
                    cmd.Parameters.Add(new SqlParameter("@level_name", revElem.level_name));
                    cmd.Parameters.Add(new SqlParameter("@level_name_standardized", revElem.level_name_standardized));
                    cmd.Parameters.Add(new SqlParameter("@phase_created", revElem.phase_created));
                    cmd.Parameters.Add(new SqlParameter("@phase_demolished", revElem.phase_demolished));
                    cmd.Parameters.Add(new SqlParameter("@comments", revElem.comments));
                    cmd.Parameters.Add(new SqlParameter("@family_name", revElem.family_name));
                    cmd.Parameters.Add(new SqlParameter("@type_name", revElem.type_name));
                    cmd.Parameters.Add(new SqlParameter("@point_1_x", revElem.point_1_x));
                    cmd.Parameters.Add(new SqlParameter("@point_1_y", revElem.point_1_y));
                    cmd.Parameters.Add(new SqlParameter("@point_1_z", revElem.point_1_z));
                    cmd.Parameters.Add(new SqlParameter("@element_glb", revElem.element_glb));
                    cmd.Parameters.Add(new SqlParameter("@width", revElem.width));
                    cmd.Parameters.Add(new SqlParameter("@length", revElem.length));
                    cmd.Parameters.Add(new SqlParameter("@height", revElem.height));
                    cmd.Parameters.Add(new SqlParameter("@parent_element_uid", revElem.parent_element_uid));
                    cmd.Parameters.Add(new SqlParameter("@is_mirrored", revElem.is_mirrored));
                    cmd.Parameters.Add(new SqlParameter("@hand_orientation", revElem.hand_orientation));

                    cmd.Parameters.Add(new SqlParameter("@tsla_scope_id", revElem.tsla_scope_id));
                    //cmd.Parameters.Add(new SqlParameter("@tsla_scope_id", revMass.GetType().GetProperty("tsla_scope_id")!=null revMass.tsla_scope_id : null? );
                    cmd.Parameters.Add(new SqlParameter("@tsla_shop", revElem.tsla_shop));
                    cmd.Parameters.Add(new SqlParameter("@tsla_program", revElem.tsla_program));
                    cmd.Parameters.Add(new SqlParameter("@tsla_name", revElem.tsla_name));
                    cmd.Parameters.Add(new SqlParameter("@tsla_dcr_id", revElem.tsla_dcr_id));
                    cmd.Parameters.Add(new SqlParameter("@tsla_space_allocation_id", revElem.tsla_space_allocation_id));



                    cmd.Parameters.Add(new SqlParameter("@point_2_x", revElem.point_2_x));
                    cmd.Parameters.Add(new SqlParameter("@point_2_y", revElem.point_2_y));
                    cmd.Parameters.Add(new SqlParameter("@point_2_z", revElem.point_2_z));
                    cmd.Parameters.Add(new SqlParameter("@aisle_two_way", revElem.aisle_two_way));
                    cmd.Parameters.Add(new SqlParameter("@a_load_width", revElem.a_load_width));
                    cmd.Parameters.Add(new SqlParameter("@b_load_width", revElem.b_load_width));
                    cmd.Parameters.Add(new SqlParameter("@wip_name", revElem.wip_name));
                    cmd.Parameters.Add(new SqlParameter("@wip_part_quantity", revElem.wip_part_quantity));
                    cmd.Parameters.Add(new SqlParameter("@wip_part_description", revElem.wip_part_description));
                    cmd.Parameters.Add(new SqlParameter("@tsla_node_type", revElem.tsla_node_type));
                    cmd.Parameters.Add(new SqlParameter("@tsla_node_is_in", revElem.tsla_node_is_in));
                    cmd.Parameters.Add(new SqlParameter("@pedestrian_width", revElem.pedestrian_width));
                    cmd.Parameters.Add(new SqlParameter("@aisle_clearance_height", revElem.aisle_clearance_height));
                    cmd.Parameters.Add(new SqlParameter("@pedestrian_clearance_height", revElem.pedestrian_clearance_height));
                    cmd.Parameters.Add(new SqlParameter("@has_pedestrian", revElem.has_pedestrian));

                    cmd.Parameters.Add(new SqlParameter("@is_double_vrc", revElem.is_double_vrc));

                    //public bool facing_flipped
                    //public bool hand_flipped}
                }

                sb.Append("\n Start cmdp \n");
                foreach (var cmdp in cmd.Parameters)
                {
                    sb.Append(cmdp.ToString());
                    sb.Append("\n");
                }
                cmd.ExecuteNonQuery();

            }

            try
            {
                sb.Append("CMD Props End \n ------------------ \n");
                foreach (PropertyInfo p in propType)
                {

                    sb.Append(p.Name + p.GetValue(propType, null));
                    sb.Append(",\n");


                }
                sb.Append("End Props:\n");
            }
            catch (Exception ex)
            {
                sb.Append("CMD Props Loop Failed");
                sb.AppendLine(ex.Message);
                return sb.ToString();
            }

            //sb.Append(String.Format("\n{0}", revElems.family_name));
            taskDialog.MainContent = sb.ToString();
            //taskDialog.MainContent = revMass.id;
            taskDialog.Show();

            //connect to sqlserver and run procedure



            return "data transfer complete!!!";
        }

    }
}