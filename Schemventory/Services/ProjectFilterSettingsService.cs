using Microsoft.Data.Sqlite;
using Schemventory.Data;
using Serilog;

namespace Schemventory.Services;

public sealed class ProjectFilterSettingsService
{
    private const string LogContext = "(ProjectFilterSettingsService)";

    private readonly DatabaseService _databaseService;

    public ProjectFilterSettingsService(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public ProjectFilterSettings GetByProjectId(Guid projectId)
    {
        using SqliteConnection connection = _databaseService.CreateConnection();
        using SqliteCommand command = connection.CreateCommand();

        command.CommandText = @"
            SELECT
                ShowMissing,
                ShowReplaced,
                ShowCollected,
                ShowIgnored
            FROM ProjectFilterSettings
            WHERE ProjectId = $projectId COLLATE NOCASE
            LIMIT 1;";

        SqliteParameter projectIdParameter = command.Parameters.Add("$projectId", SqliteType.Text);
        projectIdParameter.Value = projectId.ToString("D");

        using SqliteDataReader reader = command.ExecuteReader();

        if(!reader.Read())
        {
            Log.Debug("{LogContext} No saved filter settings found. Using defaults. ProjectId={ProjectId}", LogContext, projectId);
            return new ProjectFilterSettings();
        }

        ProjectFilterSettings settings = new()
        {
            ShowMissing = reader.GetInt32(0) != 0,
            ShowReplaced = reader.GetInt32(1) != 0,
            ShowCollected = reader.GetInt32(2) != 0,
            ShowIgnored = reader.GetInt32(3) != 0
        };

        Log.Debug("{LogContext} Project filter settings loaded. ProjectId={ProjectId}, Missing={Missing}, Replaced={Replaced}, Collected={Collected}, Ignored={Ignored}", LogContext, projectId, settings.ShowMissing, settings.ShowReplaced, settings.ShowCollected, settings.ShowIgnored);

        return settings;
    }

    public void Save(Guid projectId, ProjectFilterSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        using SqliteConnection connection = _databaseService.CreateConnection();
        using SqliteCommand command = connection.CreateCommand();

        command.CommandText = @"
            INSERT INTO ProjectFilterSettings
            (
                ProjectId,
                ShowMissing,
                ShowReplaced,
                ShowCollected,
                ShowIgnored
            )
            VALUES
            (
                $projectId,
                $showMissing,
                $showReplaced,
                $showCollected,
                $showIgnored
            )
            ON CONFLICT(ProjectId) DO UPDATE SET
                ShowMissing = excluded.ShowMissing,
                ShowReplaced = excluded.ShowReplaced,
                ShowCollected = excluded.ShowCollected,
                ShowIgnored = excluded.ShowIgnored;";

        SqliteParameter projectIdParameter = command.Parameters.Add("$projectId", SqliteType.Text);
        projectIdParameter.Value = projectId.ToString("D");

        _ = command.Parameters.AddWithValue("$showMissing", settings.ShowMissing ? 1 : 0);
        _ = command.Parameters.AddWithValue("$showReplaced", settings.ShowReplaced ? 1 : 0);
        _ = command.Parameters.AddWithValue("$showCollected", settings.ShowCollected ? 1 : 0);
        _ = command.Parameters.AddWithValue("$showIgnored", settings.ShowIgnored ? 1 : 0);

        _ = command.ExecuteNonQuery();

        Log.Debug("{LogContext} Project filter settings saved. ProjectId={ProjectId}, Missing={Missing}, Replaced={Replaced}, Collected={Collected}, Ignored={Ignored}", LogContext, projectId, settings.ShowMissing, settings.ShowReplaced, settings.ShowCollected, settings.ShowIgnored);
    }
}
