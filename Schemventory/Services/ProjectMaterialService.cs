using Microsoft.Data.Sqlite;
using Serilog;
using Schemventory.Data;

namespace Schemventory.Services;

public class ProjectMaterialService
{
    private const string LogContext = "(ProjectMaterialService)";

    private readonly DatabaseService _databaseService;

    public ProjectMaterialService(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public IReadOnlyCollection<ProjectMaterial> GetByProjectId(Guid projectId)
    {
        List<ProjectMaterial> materials = [];

        using SqliteConnection connection = _databaseService.CreateConnection();
        using SqliteCommand command = connection.CreateCommand();

        command.CommandText = @"
            SELECT
                ProjectId,
                ItemId,
                RequiredAmount,
                CollectedAmount,
                State,
                ReplacementItemId
            FROM ProjectMaterials
            WHERE ProjectId = $projectId
            ORDER BY RequiredAmount DESC, ItemId;";

        SqliteParameter projectIdParameter = command.Parameters.Add("$projectId", SqliteType.Text);
        projectIdParameter.Value = projectId.ToString("D");

        using SqliteDataReader reader = command.ExecuteReader();

        while(reader.Read())
        {
            materials.Add(new ProjectMaterial
            {
                ProjectId = reader.GetGuid(0),
                ItemId = reader.GetString(1),
                RequiredAmount = reader.GetInt64(2),
                CollectedAmount = reader.GetInt64(3),
                State = (ProjectMaterialState)reader.GetInt32(4),
                ReplacementItemId = reader.IsDBNull(5) ? null : reader.GetString(5)
            });
        }

        Log.Debug("{LogContext} Project materials loaded. ProjectId={ProjectId}, Count={Count}", LogContext, projectId, materials.Count);

        return materials;
    }

    internal void Replace(Guid projectId, IReadOnlyCollection<MaterialEntry> materials, SqliteConnection connection, SqliteTransaction transaction)
    {
        Log.Debug("{LogContext} Replacing project material set. ProjectId={ProjectId}, MaterialCount={MaterialCount}", LogContext, projectId, materials.Count);

        Dictionary<string, ExistingMaterialState> existingStates = GetExistingStates(connection, transaction, projectId);

        using(SqliteCommand deleteCommand = connection.CreateCommand())
        {
            deleteCommand.Transaction = transaction;
            deleteCommand.CommandText = "DELETE FROM ProjectMaterials WHERE ProjectId = $projectId;";

            SqliteParameter deleteProjectIdParameter = deleteCommand.Parameters.Add("$projectId", SqliteType.Text);
            deleteProjectIdParameter.Value = projectId.ToString("D");

            _ = deleteCommand.ExecuteNonQuery();
        }

        using SqliteCommand insertCommand = connection.CreateCommand();
        insertCommand.Transaction = transaction;

        insertCommand.CommandText = @"
            INSERT INTO ProjectMaterials
            (
                ProjectId,
                ItemId,
                RequiredAmount,
                CollectedAmount,
                State,
                ReplacementItemId
            )
            VALUES
            (
                $projectId,
                $itemId,
                $requiredAmount,
                $collectedAmount,
                $state,
                $replacementItemId
            );";

        SqliteParameter projectIdParameter = insertCommand.Parameters.Add("$projectId", SqliteType.Text);
        SqliteParameter itemIdParameter = insertCommand.Parameters.Add("$itemId", SqliteType.Text);
        SqliteParameter requiredAmountParameter = insertCommand.Parameters.Add("$requiredAmount", SqliteType.Integer);
        SqliteParameter collectedAmountParameter = insertCommand.Parameters.Add("$collectedAmount", SqliteType.Integer);
        SqliteParameter stateParameter = insertCommand.Parameters.Add("$state", SqliteType.Integer);
        SqliteParameter replacementItemIdParameter = insertCommand.Parameters.Add("$replacementItemId", SqliteType.Text);

        projectIdParameter.Value = projectId.ToString("D");

        foreach(MaterialEntry material in materials)
        {
            ExistingMaterialState? existingState = existingStates.TryGetValue(material.Id, out ExistingMaterialState? value) ? value : null;
            ProjectMaterialState state = existingState?.State ?? ProjectMaterialState.Missing;
            var replacementItemId = state is ProjectMaterialState.Replaced or ProjectMaterialState.Collected ? existingState?.ReplacementItemId : null;
            var collectedAmount = state == ProjectMaterialState.Collected ? material.BlocksTotal : Math.Min(existingState?.CollectedAmount ?? 0, material.BlocksTotal);

            if(state is ProjectMaterialState.Replaced or ProjectMaterialState.Ignored)
                collectedAmount = 0;

            itemIdParameter.Value = material.Id;
            requiredAmountParameter.Value = material.BlocksTotal;
            collectedAmountParameter.Value = collectedAmount;
            stateParameter.Value = (int)state;
            replacementItemIdParameter.Value = (object?)replacementItemId ?? DBNull.Value;

            _ = insertCommand.ExecuteNonQuery();
        }

        Log.Debug("{LogContext} Project material set replaced. ProjectId={ProjectId}, MaterialCount={MaterialCount}", LogContext, projectId, materials.Count);
    }

    public void UpdateCollectedAmount(Guid projectId, string itemId, long collectedAmount)
    {
        if(collectedAmount < 0)
            throw new ArgumentOutOfRangeException(nameof(collectedAmount));

        using SqliteConnection connection = _databaseService.CreateConnection();
        using SqliteCommand command = connection.CreateCommand();

        command.CommandText = @"
            UPDATE ProjectMaterials
            SET CollectedAmount = MIN($collectedAmount, RequiredAmount),
                State = CASE
                    WHEN $collectedAmount >= RequiredAmount THEN $collectedState
                    WHEN ReplacementItemId IS NOT NULL THEN $replacedState
                    ELSE $missingState
                END
            WHERE ProjectId = $projectId
            AND ItemId = $itemId;";

        SqliteParameter projectIdParameter = command.Parameters.Add("$projectId", SqliteType.Text);
        projectIdParameter.Value = projectId.ToString("D");

        _ = command.Parameters.AddWithValue("$itemId", itemId);
        _ = command.Parameters.AddWithValue("$collectedAmount", collectedAmount);
        _ = command.Parameters.AddWithValue("$collectedState", (int)ProjectMaterialState.Collected);
        _ = command.Parameters.AddWithValue("$replacedState", (int)ProjectMaterialState.Replaced);
        _ = command.Parameters.AddWithValue("$missingState", (int)ProjectMaterialState.Missing);

        EnsureMaterialUpdated(command.ExecuteNonQuery(), projectId, itemId);

        Log.Debug("{LogContext} Collected amount updated. ProjectId={ProjectId}, ItemId={ItemId}, CollectedAmount={CollectedAmount}", LogContext, projectId, itemId, collectedAmount);
    }

    public void MarkCompleted(Guid projectId, string itemId)
    {
        SetState(projectId, itemId, ProjectMaterialState.Collected);
    }

    public void MarkMissing(Guid projectId, string itemId)
    {
        SetState(projectId, itemId, ProjectMaterialState.Missing);
    }

    public void MarkIgnored(Guid projectId, string itemId)
    {
        SetState(projectId, itemId, ProjectMaterialState.Ignored);
    }

    public void MarkReplaced(Guid projectId, string itemId, string replacementItemId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(replacementItemId);

        if(string.Equals(itemId, replacementItemId, StringComparison.Ordinal))
            throw new ArgumentException("The replacement material must be different from the original material.", nameof(replacementItemId));

        SetState(projectId, itemId, ProjectMaterialState.Replaced, replacementItemId);
    }

    private void SetState(Guid projectId, string itemId, ProjectMaterialState state, string? replacementItemId = null)
    {
        if(state == ProjectMaterialState.Replaced && string.IsNullOrWhiteSpace(replacementItemId))
            throw new ArgumentException("A replacement material is required for the replaced state.", nameof(replacementItemId));

        using SqliteConnection connection = _databaseService.CreateConnection();
        using SqliteCommand command = connection.CreateCommand();

        command.CommandText = @"
            UPDATE ProjectMaterials
            SET State = $state,
                ReplacementItemId = CASE
                    WHEN $state = $collectedState THEN ReplacementItemId
                    ELSE $replacementItemId
                END,
                CollectedAmount = CASE
                    WHEN $state = $collectedState THEN RequiredAmount
                    ELSE 0
                END
            WHERE ProjectId = $projectId
            AND ItemId = $itemId;";

        SqliteParameter projectIdParameter = command.Parameters.Add("$projectId", SqliteType.Text);
        projectIdParameter.Value = projectId.ToString("D");

        _ = command.Parameters.AddWithValue("$itemId", itemId);
        _ = command.Parameters.AddWithValue("$state", (int)state);
        _ = command.Parameters.AddWithValue("$collectedState", (int)ProjectMaterialState.Collected);
        _ = command.Parameters.AddWithValue("$replacementItemId", state == ProjectMaterialState.Replaced ? replacementItemId! : DBNull.Value);

        EnsureMaterialUpdated(command.ExecuteNonQuery(), projectId, itemId);

        Log.Debug("{LogContext} Material state changed. ProjectId={ProjectId}, ItemId={ItemId}, State={State}, ReplacementItemId={ReplacementItemId}", LogContext, projectId, itemId, state, replacementItemId);
    }

    private static Dictionary<string, ExistingMaterialState> GetExistingStates(SqliteConnection connection, SqliteTransaction transaction, Guid projectId)
    {
        Dictionary<string, ExistingMaterialState> states = new(StringComparer.Ordinal);

        using SqliteCommand command = connection.CreateCommand();
        command.Transaction = transaction;

        command.CommandText = @"
            SELECT ItemId, CollectedAmount, State, ReplacementItemId
            FROM ProjectMaterials
            WHERE ProjectId = $projectId;";

        SqliteParameter projectIdParameter = command.Parameters.Add("$projectId", SqliteType.Text);
        projectIdParameter.Value = projectId.ToString("D");

        using SqliteDataReader reader = command.ExecuteReader();

        while(reader.Read())
        {
            states[reader.GetString(0)] = new ExistingMaterialState
            {
                CollectedAmount = reader.GetInt64(1),
                State = (ProjectMaterialState)reader.GetInt32(2),
                ReplacementItemId = reader.IsDBNull(3) ? null : reader.GetString(3)
            };
        }

        return states;
    }

    private static void EnsureMaterialUpdated(int affectedRows, Guid projectId, string itemId)
    {
        if(affectedRows != 0)
            return;

        Log.Warning("{LogContext} Material update failed because the material does not exist. ProjectId={ProjectId}, ItemId={ItemId}", LogContext, projectId, itemId);
        throw new InvalidOperationException($"Material '{itemId}' does not exist in project '{projectId}'.");
    }

    private sealed class ExistingMaterialState
    {
        public long CollectedAmount { get; init; }
        public ProjectMaterialState State { get; init; }
        public string? ReplacementItemId { get; init; }
    }
}
