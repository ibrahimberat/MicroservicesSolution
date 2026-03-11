using LogService.Domain.Entities;

namespace LogService.Domain.Interfaces
{
    public interface ILogRepository
    {
        Task<LogEntry?> GetByIdAsync(Guid id);
        Task<IEnumerable<LogEntry>> GetAllAsync();
        Task<IEnumerable<LogEntry>> GetByServiceAsync(string serviceName);
        Task<IEnumerable<LogEntry>> GetByLevelAsync(string level);
        Task<IEnumerable<LogEntry>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<LogEntry> AddAsync(LogEntry logEntry);
        Task AddRangeAsync(IEnumerable<LogEntry> logEntries);
        Task<bool> DeleteOldLogsAsync(int daysToKeep);
    }
}