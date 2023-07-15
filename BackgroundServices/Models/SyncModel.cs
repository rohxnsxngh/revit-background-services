using System;
using System.Collections.Generic;
using System.Linq;

namespace BackgroundServices.Models;

public class SyncModel
{
    public Guid job_id { get; set; }

    public string user_name { get; set; } = "scheduled task";
    public List<string> model_name { get; set; } = new List<string>();
    public DateTime? date_created { get; set; }

    public DateTime? date_completed { get; set; } = null;
    public int status { get; set; }

    public List<string> failed_model { get; set; } = new List<string>();

}