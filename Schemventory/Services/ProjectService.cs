using Microsoft.Data.Sqlite;
using Schemventory.Data;

namespace Schemventory.Services;

internal class ProjectService
{
    private readonly DatabaseService _databaseService;

    public ProjectService(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    internal void Add(Project project, SqliteConnection connection, SqliteTransaction transaction)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.Transaction = transaction;

        command.CommandText = @"
        INSERT INTO Projects
        (
            Id,
            Name,
            SchematicPath,
            CreatedAtUtc,
            LastOpenedAtUtc
        )
        VALUES
        (
            $id,
            $name,
            $schematicPath,
            $createdAtUtc,
            $lastOpenedAtUtc
        );";

        SqliteParameter idParameter = command.Parameters.Add("$id", SqliteType.Text);
        idParameter.Value = project.Id.ToString("D");

        _ = command.Parameters.AddWithValue("$name", project.Name);
        _ = command.Parameters.AddWithValue("$schematicPath", project.SchematicPath);
        _ = command.Parameters.AddWithValue("$createdAtUtc", project.CreatedAtUtc.ToString("O"));
        _ = command.Parameters.AddWithValue("$lastOpenedAtUtc", project.LastOpenedAtUtc.ToString("O"));

        _ = command.ExecuteNonQuery();
    }

    internal void Update(Project project, SqliteConnection connection, SqliteTransaction transaction)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.Transaction = transaction;

        command.CommandText = @"
        UPDATE Projects
        SET
            Name = $name,
            SchematicPath = $schematicPath,
            LastOpenedAtUtc = $lastOpenedAtUtc
        WHERE Id = $id;";

        SqliteParameter idParameter = command.Parameters.Add("$id", SqliteType.Text);
        idParameter.Value = project.Id.ToString("D");

        _ = command.Parameters.AddWithValue("$name", project.Name);
        _ = command.Parameters.AddWithValue("$schematicPath", project.SchematicPath);
        _ = command.Parameters.AddWithValue("$lastOpenedAtUtc", project.LastOpenedAtUtc.ToString("O"));

        var affectedRows = command.ExecuteNonQuery();

        if(affectedRows == 0)
            throw new InvalidOperationException($"Project '{project.Id}' does not exist.");
    }

    public void Delete(Guid projectId)
    {
        using SqliteConnection connection = _databaseService.CreateConnection();
        using SqliteCommand command = connection.CreateCommand();

        command.CommandText = @"
            DELETE FROM Projects
            WHERE Id = $id;";

        SqliteParameter idParameter = command.Parameters.Add("$id", SqliteType.Text);
        idParameter.Value = projectId.ToString("D");

        var affectedRows = command.ExecuteNonQuery();

        if(affectedRows == 0)
            throw new InvalidOperationException($"Project '{projectId}' does not exist.");
    }

    public IReadOnlyCollection<Project> GetAll()
    {
        List<Project> projects = [];

        using SqliteConnection connection = _databaseService.CreateConnection();
        using SqliteCommand command = connection.CreateCommand();

        command.CommandText = @"
            SELECT
                Id,
                Name,
                SchematicPath,
                CreatedAtUtc,
                LastOpenedAtUtc
            FROM Projects
            ORDER BY LastOpenedAtUtc DESC;";

        using SqliteDataReader reader = command.ExecuteReader();

        while(reader.Read())
        {
            Project project = new()
            {
                Id = reader.GetGuid(0),
                Name = reader.GetString(1),
                SchematicPath = reader.GetString(2),
                CreatedAtUtc = reader.GetDateTime(3),
                LastOpenedAtUtc = reader.GetDateTime(4)
            };

            projects.Add(project);
        }

        return projects;
    }
}
