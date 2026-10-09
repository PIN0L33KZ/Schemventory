using Microsoft.Data.Sqlite;
using Serilog;
using Schemventory.Data;

namespace Schemventory.Services;

internal sealed class ProjectPersistenceService
{
    private const string LogContext = "(ProjectPersistenceService)";

    private readonly DatabaseService _databaseService;
    private readonly ProjectService _projectService;
    private readonly ProjectMaterialService _projectMaterialService;

    public ProjectPersistenceService(DatabaseService databaseService)
    {
        _databaseService = databaseService;
        _projectService = new ProjectService(databaseService);
        _projectMaterialService = new ProjectMaterialService(databaseService);
    }

    public void Create(Project project, IReadOnlyCollection<MaterialEntry> materials)
    {
        Log.Debug("{LogContext} Starting project creation transaction. ProjectId={ProjectId}, MaterialCount={MaterialCount}", LogContext, project.Id, materials.Count);

        using SqliteConnection connection = _databaseService.CreateConnection();
        using SqliteTransaction transaction = connection.BeginTransaction();

        try
        {
            _projectService.Add(project, connection, transaction);
            _projectMaterialService.Replace(project.Id, materials, connection, transaction);

            transaction.Commit();

            Log.Debug("{LogContext} Project creation transaction committed. ProjectId={ProjectId}", LogContext, project.Id);
        }
        catch(Exception exception)
        {
            transaction.Rollback();
            Log.Error(exception, "{LogContext} Project creation transaction failed and was rolled back. ProjectId={ProjectId}", LogContext, project.Id);
            throw;
        }
    }

    public void Update(Project project, IReadOnlyCollection<MaterialEntry>? materials = null)
    {
        Log.Debug("{LogContext} Starting project update transaction. ProjectId={ProjectId}, ReplaceMaterials={ReplaceMaterials}", LogContext, project.Id, materials is not null);

        using SqliteConnection connection = _databaseService.CreateConnection();
        using SqliteTransaction transaction = connection.BeginTransaction();

        try
        {
            _projectService.Update(project, connection, transaction);

            if(materials is not null)
                _projectMaterialService.Replace(project.Id, materials, connection, transaction);

            transaction.Commit();

            Log.Debug("{LogContext} Project update transaction committed. ProjectId={ProjectId}", LogContext, project.Id);
        }
        catch(Exception exception)
        {
            transaction.Rollback();
            Log.Error(exception, "{LogContext} Project update transaction failed and was rolled back. ProjectId={ProjectId}", LogContext, project.Id);
            throw;
        }
    }
}
