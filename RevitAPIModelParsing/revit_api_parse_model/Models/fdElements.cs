using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace revit_api_parse_model.models
{
    public class fd_element
    {
        // REVIT PARAMETERS
        public string element_uid { get; set; } = string.Empty; //element uid
        public int element_id { get; set; } //element id that is searchable in revit
        public string sync_id { get; set; } = string.Empty;
        public string design_option_set { get; set; } = string.Empty;
        public string design_option_name { get; set; } = string.Empty;
        public bool design_option_is_primary { get; set; }
        public string category_name { get; set; } = string.Empty;
        public string? workset_name { get; set; }
        public string level_name { get; set; } = string.Empty;
        public string? level_name_standardized { get; set; }
        public string phase_created { get; set; } = string.Empty;
        public string? phase_demolished { get; set; }
        public string? comments { get; set; }
        public string? family_name { get; set; }
        public string? type_name { get; set; }
        public double? point_1_x { get; set; }
        public double? point_1_y { get; set; }
        public double? point_1_z { get; set; }
        public string? element_glb { get; set; }
        public double? width { get; set; }
        public double? length { get; set; }
        public double? height { get; set; }
        public bool is_mirrored { get; set; }
        public bool facing_flipped { get; set; }
        public bool hand_flipped { get; set; }
        public string? hand_orientation { get; set; }
        public string? parent_element_uid { get; set; }

        // TSLA PARAMETERS
        public string? tsla_space_allocation_id { get; set; }
        public string? tsla_shop { get; set; }
        public string? tsla_program { get; set; }
        public string? tsla_name { get; set; }
        public string? tsla_dcr_id { get; set; }

        // MASS ELEMENTS (OLDER)
        public string? tsla_scope_id { get; set; }

        // NURSE CALL DEVICES
        public double? point_2_x { get; set; }
        public double? point_2_y { get; set; }
        public double? point_2_z { get; set; }
        public bool? aisle_two_way { get; set; }
        public double? a_load_width { get; set; }
        public double? b_load_width { get; set; }
        public string? wip_name { get; set; }
        public int? wip_part_quantity { get; set; }
        public string? wip_part_description { get; set; }
        public string? tsla_node_type { get; set; }
        public bool? tsla_node_is_in { get; set; }
        public float? pedestrian_width { get; set; }
        public float? aisle_clearance_height { get; set; }
        public float? pedestrian_clearance_height { get; set; }
        public bool? has_pedestrian { get; set; }

        // GENERICA MODELS
        public bool? is_double_vrc { get; set; }
    }

}
