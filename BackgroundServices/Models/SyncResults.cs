using System.Collections.Generic;

namespace BackgroundServices.Models;

public class SyncResult
{
    public int Result { get; set; }
    public List<string> FailedModelNames { get; set; } = new List<string>();
}