using Dapper;
using Microsoft.Data.Sqlite;

namespace BlazorLoggingDemo.Data;

public static class DatabaseInitializer
{
    public static void InitializeDatabase(string connectionString)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        
        // Create Logs table if it doesn't exist
        connection.Execute(@"
            CREATE TABLE IF NOT EXISTS Logs (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Timestamp TEXT NOT NULL,
                Level VARCHAR(128) NOT NULL,
                Message TEXT,
                Exception TEXT,
                Properties TEXT
            )
        ");
        
        // Optionally add indices for better query performance
        connection.Execute("CREATE INDEX IF NOT EXISTS IX_Logs_Timestamp ON Logs (Timestamp)");
        connection.Execute("CREATE INDEX IF NOT EXISTS IX_Logs_Level ON Logs (Level)");
    }
}