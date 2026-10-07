using Microsoft.Data.Sqlite;
using Schemventory.Data;

namespace Schemventory.Services;

internal sealed class ProjectPersistenceService
{
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
        using SqliteConnection connection = _databaseService.CreateConnection();
        using SqliteTransaction transaction = connection.BeginTransaction();

        try
        {
            _projectService.Add(project, connection, transaction);
            _projectMaterialService.Replace(project.Id, materials, connection, transaction);

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public void Update(Project project, IReadOnlyCollection<MaterialEntry>? materials = null)
    {
        using SqliteConnection connection = _databaseService.CreateConnection();
        using SqliteTransaction transaction = connection.BeginTransaction();

        try
        {
            _projectService.Update(project, connection, transaction);

            if(materials is not null)
                _projectMaterialService.Replace(project.Id, materials, connection, transaction);

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}