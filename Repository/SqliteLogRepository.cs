using Dapper;
using Microsoft.Data.Sqlite;

namespace BlazorLoggingDemo.Repository;

public class LogEntry
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string Level { get; set; }
    public string Message { get; set; }
    public string Exception { get; set; }
    public string Properties { get; set; }
}

public interface ILogRepository
{
    Task<IEnumerable<LogEntry>> GetRecentLogsAsync(int count = 100);
    Task<IEnumerable<LogEntry>> GetLogsByLevelAsync(string level, DateTime startDate, DateTime endDate);
    Task<int> GetLogCountAsync();
}

public class SqliteLogRepository : ILogRepository
{
    private readonly string _connectionString;
    
    public SqliteLogRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("LogsConnection");
    }
    
    public async Task<IEnumerable<LogEntry>> GetRecentLogsAsync(int count = 100)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        
        return await connection.QueryAsync<LogEntry>(
            "SELECT Id, Timestamp, Level, RenderedMessage as Message, Exception, Properties " +
            "FROM Logs ORDER BY Timestamp DESC LIMIT @Count",
            new { Count = count });
    }
    
    public async Task<IEnumerable<LogEntry>> GetLogsByLevelAsync(string level, DateTime startDate, DateTime endDate)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        
        return await connection.QueryAsync<LogEntry>(
            "SELECT Id, Timestamp, Level, RenderedMessage as Message, Exception, Properties " +
            "FROM Logs WHERE Level = @Level AND Timestamp BETWEEN @Start AND @End " +
            "ORDER BY Timestamp DESC",
            new { Level = level, Start = startDate.ToString("o"), End = endDate.ToString("o") });
    }
    
    public async Task<int> GetLogCountAsync()
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        
        return await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Logs");
    }
}