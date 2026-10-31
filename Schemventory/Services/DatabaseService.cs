using Microsoft.Data.Sqlite;
using Schemventory.App;
using Serilog;

namespace Schemventory.Services;

public class DatabaseService
{
    private const string LogContext = "(DatabaseService)";

    private readonly string _databaseFilePath = Constants.AppDatabasePath;

    public DatabaseService()
    {
        Helper.EnsureAppDataPath();
        Log.Debug("{LogContext} Database path: {DatabasePath}", LogContext, _databaseFilePath);
    }

    public SqliteConnection CreateConnection()
    {
        SqliteConnectionStringBuilder connectionStringBuilder = new()
        {
            DataSource = _databaseFilePath,
            Mode = SqliteOpenMode.ReadWriteCreate
        };

        SqliteConnection connection = new(connectionStringBuilder.ToString());

        connection.Open();

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText = @"
            PRAGMA foreign_keys = ON;
            PRAGMA synchronous = NORMAL;";

        _ = command.ExecuteNonQuery();

        return connection;
    }

    public void InitializeDatabase()
    {
        Log.Debug("{LogContext} Initialising database.", LogContext);

        using SqliteConnection connection = CreateConnection();

        ConfigureDatabase(connection);
        CreateProjectsTable(connection);
        MigrateProjectsTable(connection);
        CreateProjectMaterialsTable(connection);
        MigrateProjectMaterialsTable(connection);
        CreateProjectFilterSettingsTable(connection);
        NormalizeProjectIds(connection);

        Log.Debug("{LogContext} Database initialisation completed.", LogContext);
    }

    private static void ConfigureDatabase(SqliteConnection connection)
    {
        using SqliteCommand command = connection.CreateCommand();

        command.CommandText = "PRAGMA journal_mode = WAL;";

        _ = command.ExecuteScalar();

        Log.Debug("{LogContext} WAL journal mode configured.", LogContext);
    }

    private static void CreateProjectsTable(SqliteConnection connection)
    {
        var query = @"
            CREATE TABLE IF NOT EXISTS Projects
            (
                Id TEXT NOT NULL PRIMARY KEY,
                Name TEXT NOT NULL,
                SchematicPath TEXT NULL,
                CreatedAtUtc TEXT NOT NULL,
                LastOpenedAtUtc TEXT NOT NULL
            );";

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = query;
        _ = command.ExecuteNonQuery();
    }

    private static void MigrateProjectsTable(SqliteConnection connection)
    {
        if(ColumnAllowsNull(connection, "Projects", "SchematicPath"))
            return;

        Log.Information("{LogContext} Migrating Projects.SchematicPath to nullable.", LogContext);

        SetForeignKeysEnabled(connection, false);

        try
        {
            using SqliteTransaction transaction = connection.BeginTransaction();

            try
            {
                using(SqliteCommand createCommand = connection.CreateCommand())
                {
                    createCommand.Transaction = transaction;
                    createCommand.CommandText = @"
                        CREATE TABLE Projects_New
                        (
                            Id TEXT NOT NULL PRIMARY KEY,
                            Name TEXT NOT NULL,
                            SchematicPath TEXT NULL,
                            CreatedAtUtc TEXT NOT NULL,
                            LastOpenedAtUtc TEXT NOT NULL
                        );";

                    _ = createCommand.ExecuteNonQuery();
                }

                using(SqliteCommand copyCommand = connection.CreateCommand())
                {
                    copyCommand.Transaction = transaction;
                    copyCommand.CommandText = @"
                        INSERT INTO Projects_New
                        (
                            Id,
                            Name,
                            SchematicPath,
                            CreatedAtUtc,
                            LastOpenedAtUtc
                        )
                        SELECT
                            Id,
                            Name,
                            SchematicPath,
                            CreatedAtUtc,
                            LastOpenedAtUtc
                        FROM Projects;";

                    _ = copyCommand.ExecuteNonQuery();
                }

                using(SqliteCommand dropCommand = connection.CreateCommand())
                {
                    dropCommand.Transaction = transaction;
                    dropCommand.CommandText = "DROP TABLE Projects;";

                    _ = dropCommand.ExecuteNonQuery();
                }

                using(SqliteCommand renameCommand = connection.CreateCommand())
                {
                    renameCommand.Transaction = transaction;
                    renameCommand.CommandText = "ALTER TABLE Projects_New RENAME TO Projects;";

                    _ = renameCommand.ExecuteNonQuery();
                }

                transaction.Commit();

                Log.Information("{LogContext} Projects.SchematicPath migration completed.", LogContext);
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch(Exception exception)
        {
            Log.Error(exception, "{LogContext} Failed to migrate Projects.SchematicPath to nullable.", LogContext);
            throw;
        }
        finally
        {
            SetForeignKeysEnabled(connection, true);
        }
    }

    private static void CreateProjectMaterialsTable(SqliteConnection connection)
    {
        var query = @"
            CREATE TABLE IF NOT EXISTS ProjectMaterials
            (
                ProjectId TEXT NOT NULL,
                ItemId TEXT NOT NULL,
                RequiredAmount INTEGER NOT NULL,
                CollectedAmount INTEGER NOT NULL DEFAULT 0,
                State INTEGER NOT NULL DEFAULT 0,
                ReplacementItemId TEXT NULL,

                PRIMARY KEY(ProjectId, ItemId),

                FOREIGN KEY(ProjectId)
                    REFERENCES Projects(Id)
                    ON DELETE CASCADE
            );";

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = query;
        _ = command.ExecuteNonQuery();
    }

    private static void MigrateProjectMaterialsTable(SqliteConnection connection)
    {
        var stateColumnAdded = false;

        if(!ColumnExists(connection, "ProjectMaterials", "State"))
        {
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = "ALTER TABLE ProjectMaterials ADD COLUMN State INTEGER NOT NULL DEFAULT 0;";

            _ = command.ExecuteNonQuery();
            stateColumnAdded = true;

            Log.Information("{LogContext} Added ProjectMaterials.State column.", LogContext);
        }

        if(!ColumnExists(connection, "ProjectMaterials", "ReplacementItemId"))
        {
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = "ALTER TABLE ProjectMaterials ADD COLUMN ReplacementItemId TEXT NULL;";

            _ = command.ExecuteNonQuery();

            Log.Information("{LogContext} Added ProjectMaterials.ReplacementItemId column.", LogContext);
        }

        if(stateColumnAdded)
        {
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE ProjectMaterials
                SET State = $collectedState
                WHERE CollectedAmount >= RequiredAmount;";

            _ = command.Parameters.AddWithValue("$collectedState", (int)ProjectMaterialState.Collected);
            _ = command.ExecuteNonQuery();

            Log.Information("{LogContext} Existing material states migrated.", LogContext);
        }
    }

    private static void CreateProjectFilterSettingsTable(SqliteConnection connection)
    {
        using SqliteCommand command = connection.CreateCommand();

        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS ProjectFilterSettings
            (
                ProjectId TEXT NOT NULL PRIMARY KEY,
                ShowMissing INTEGER NOT NULL DEFAULT 1,
                ShowReplaced INTEGER NOT NULL DEFAULT 1,
                ShowCollected INTEGER NOT NULL DEFAULT 0,
                ShowIgnored INTEGER NOT NULL DEFAULT 0,

                FOREIGN KEY(ProjectId)
                    REFERENCES Projects(Id)
                    ON DELETE CASCADE
            );";

        _ = command.ExecuteNonQuery();
    }

    private static void NormalizeProjectIds(SqliteConnection connection)
    {
        using(SqliteCommand duplicateProjectCommand = connection.CreateCommand())
        {
            duplicateProjectCommand.CommandText = @"
                SELECT LOWER(Id)
                FROM Projects
                GROUP BY LOWER(Id)
                HAVING COUNT(*) > 1
                LIMIT 1;";

            if(duplicateProjectCommand.ExecuteScalar() is not null)
                throw new InvalidOperationException("Project IDs cannot be normalised because duplicate IDs with different casing exist.");
        }

        using(SqliteCommand duplicateMaterialCommand = connection.CreateCommand())
        {
            duplicateMaterialCommand.CommandText = @"
                SELECT LOWER(ProjectId)
                FROM ProjectMaterials
                GROUP BY LOWER(ProjectId), ItemId
                HAVING COUNT(*) > 1
                LIMIT 1;";

            if(duplicateMaterialCommand.ExecuteScalar() is not null)
                throw new InvalidOperationException("Project material IDs cannot be normalised because duplicate material keys with different project ID casing exist.");
        }

        using(SqliteCommand duplicateFilterSettingsCommand = connection.CreateCommand())
        {
            duplicateFilterSettingsCommand.CommandText = @"
                SELECT LOWER(ProjectId)
                FROM ProjectFilterSettings
                GROUP BY LOWER(ProjectId)
                HAVING COUNT(*) > 1
                LIMIT 1;";

            if(duplicateFilterSettingsCommand.ExecuteScalar() is not null)
                throw new InvalidOperationException("Project filter settings IDs cannot be normalised because duplicate project IDs with different casing exist.");
        }

        SetForeignKeysEnabled(connection, false);

        try
        {
            using SqliteTransaction transaction = connection.BeginTransaction();

            try
            {
                using(SqliteCommand materialCommand = connection.CreateCommand())
                {
                    materialCommand.Transaction = transaction;
                    materialCommand.CommandText = @"
                        UPDATE ProjectMaterials
                        SET ProjectId = LOWER(ProjectId)
                        WHERE ProjectId <> LOWER(ProjectId);";

                    var affectedMaterials = materialCommand.ExecuteNonQuery();

                    if(affectedMaterials > 0)
                        Log.Information("{LogContext} Normalised project material IDs. Count={Count}", LogContext, affectedMaterials);
                }

                using(SqliteCommand filterSettingsCommand = connection.CreateCommand())
                {
                    filterSettingsCommand.Transaction = transaction;
                    filterSettingsCommand.CommandText = @"
                        UPDATE ProjectFilterSettings
                        SET ProjectId = LOWER(ProjectId)
                        WHERE ProjectId <> LOWER(ProjectId);";

                    var affectedFilterSettings = filterSettingsCommand.ExecuteNonQuery();

                    if(affectedFilterSettings > 0)
                        Log.Information("{LogContext} Normalised project filter settings IDs. Count={Count}", LogContext, affectedFilterSettings);
                }

                using(SqliteCommand projectCommand = connection.CreateCommand())
                {
                    projectCommand.Transaction = transaction;
                    projectCommand.CommandText = @"
                        UPDATE Projects
                        SET Id = LOWER(Id)
                        WHERE Id <> LOWER(Id);";

                    var affectedProjects = projectCommand.ExecuteNonQuery();

                    if(affectedProjects > 0)
                        Log.Information("{LogContext} Normalised project IDs. Count={Count}", LogContext, affectedProjects);
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        finally
        {
            SetForeignKeysEnabled(connection, true);
        }

        using SqliteCommand foreignKeyCheckCommand = connection.CreateCommand();
        foreignKeyCheckCommand.CommandText = "PRAGMA foreign_key_check;";

        using SqliteDataReader reader = foreignKeyCheckCommand.ExecuteReader();

        if(reader.Read())
            throw new InvalidOperationException("Foreign key validation failed after normalising project IDs.");
    }

    private static bool ColumnExists(SqliteConnection connection, string tableName, string columnName)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = $"PRAGMA table_info({tableName});";

        using SqliteDataReader reader = command.ExecuteReader();

        while(reader.Read())
        {
            if(string.Equals(reader.GetString(1), columnName, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }

    private static bool ColumnAllowsNull(SqliteConnection connection, string tableName, string columnName)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = $"PRAGMA table_info({tableName});";

        using SqliteDataReader reader = command.ExecuteReader();

        while(reader.Read())
        {
            if(!string.Equals(reader.GetString(1), columnName, StringComparison.OrdinalIgnoreCase))
                continue;

            return reader.GetInt32(3) == 0;
        }

        return false;
    }

    private static void SetForeignKeysEnabled(SqliteConnection connection, bool enabled)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = enabled ? "PRAGMA foreign_keys = ON;" : "PRAGMA foreign_keys = OFF;";

        _ = command.ExecuteNonQuery();
    }
}
