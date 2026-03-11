using System.Collections.Generic;

namespace LogService.Domain.Entities
{
    public class LogEntry
    {
        public Guid Id { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Exception { get; set; }
        public string? StackTrace { get; set; }
        public string? UserId { get; set; }
        public string? IpAddress { get; set; }
        public string? Path { get; set; }
        public string? Method { get; set; }
        public int? StatusCode { get; set; }
        public long? Duration { get; set; }
        public string? Properties { get; set; } // JSON olarak saklanacak
        public DateTime Timestamp { get; set; }
    }

    public enum LogLevel
    {
        Info,
        Warning,
        Error,
        Critical
    }
}