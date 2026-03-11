using Microsoft.EntityFrameworkCore;
using LogService.Domain.Entities;
using LogService.Domain.Interfaces;
using LogService.Infrastructure.Data;

namespace LogService.Infrastructure.Repositories
{
    public class LogRepository : ILogRepository
    {
        private readonly LogDbContext _context;

        public LogRepository(LogDbContext context)
        {
            _context = context;
        }

        public async Task<LogEntry?> GetByIdAsync(Guid id)
        {
            return await _context.Logs.FindAsync(id);
        }

        public async Task<IEnumerable<LogEntry>> GetAllAsync()
        {
            return await _context.Logs
                .OrderByDescending(l => l.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<LogEntry>> GetByServiceAsync(string serviceName)
        {
            return await _context.Logs
                .Where(l => l.ServiceName == serviceName)
                .OrderByDescending(l => l.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<LogEntry>> GetByLevelAsync(string level)
        {
            return await _context.Logs
                .Where(l => l.Level == level)
                .OrderByDescending(l => l.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<LogEntry>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Logs
                .Where(l => l.Timestamp >= startDate && l.Timestamp <= endDate)
                .OrderByDescending(l => l.Timestamp)
                .ToListAsync();
        }

        public async Task<LogEntry> AddAsync(LogEntry logEntry)
        {
            await _context.Logs.AddAsync(logEntry);
            await _context.SaveChangesAsync();
            return logEntry;
        }

        public async Task AddRangeAsync(IEnumerable<LogEntry> logEntries)
        {
            await _context.Logs.AddRangeAsync(logEntries);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteOldLogsAsync(int daysToKeep)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-daysToKeep);
            var oldLogs = await _context.Logs
                .Where(l => l.Timestamp < cutoffDate)
                .ToListAsync();

            if (oldLogs.Any())
            {
                _context.Logs.RemoveRange(oldLogs);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}