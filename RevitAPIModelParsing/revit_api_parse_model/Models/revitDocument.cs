using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace revit_api_parse_model.models
{
    public class revit_document
    {
        public string guid_project { get; set; } = string.Empty;
        public string doc_id { get; set; } = string.Empty;
        public string model_name { get; set; } = string.Empty;
        public string model_desc { get; set; } = string.Empty;
        public string model_path { get; set; } = string.Empty;
        public DateTime sync_time { get; set; }
        public string user_name { get; set; } = string.Empty;
        public string user_machine { get; set; } = string.Empty;
        public string sync_id { get; set; } = string.Empty;
    }
}