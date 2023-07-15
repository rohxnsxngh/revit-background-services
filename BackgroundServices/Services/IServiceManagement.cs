using BackgroundServices.Services;
using BackgroundServices.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public interface IServiceManagement
{
    Task<SyncResult> SyncIndividualModelsToDatabase(List<string> revitModelName, CancellationToken cancellationToken);

    void SyncRevitMasterFilesWithRevitBatchProcesssingStructure(CancellationToken cancellationToken);

    void UpdateDatabase(CancellationToken cancellationToken);

    void DeleteLocalLoggingFile(CancellationToken cancellationToken);

    Task ServiceDatabase(List<string> revitModelNames, string? userName, CancellationToken cancellationToken);

    void HangfireServiceSQLiteTracking(CancellationToken cancellationToken);
}