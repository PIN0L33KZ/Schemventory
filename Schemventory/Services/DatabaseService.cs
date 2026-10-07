using Microsoft.Data.Sqlite;
using Schemventory.App;

namespace Schemventory.Services;

public class DatabaseService
{
    private readonly string _databaseFilePath = Constants.AppDatabasePath;

    public DatabaseService()
    {
        Helper.EnsureAppDataPath();
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

        command.CommandText = @"PRAGMA foreign_keys = ON;";
        _ = command.ExecuteNonQuery();

        return connection;
    }

    public void InitializeDatabase()
    {
        using SqliteConnection connection = CreateConnection();

        CreateProjectsTable(connection);
        CreateProjectMaterialsTable(connection);
        MigrateProjectMaterialsTable(connection);
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
        }

        if(!ColumnExists(connection, "ProjectMaterials", "ReplacementItemId"))
        {
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = "ALTER TABLE ProjectMaterials ADD COLUMN ReplacementItemId TEXT NULL;";
            _ = command.ExecuteNonQuery();
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
