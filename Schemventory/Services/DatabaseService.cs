using Microsoft.Data.Sqlite;
using Serilog;
using Schemventory.App;

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
        CreateProjectMaterialsTable(connection);
        MigrateProjectMaterialsTable(connection);

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
                SchematicPath TEXT NOT NULL,
                CreatedAtUtc TEXT NOT NULL,
                LastOpenedAtUtc TEXT NOT NULL
            );";

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = query;
        _ = command.ExecuteNonQuery();
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
}
