using MediatR;
using LogService.Domain.Entities;
using LogService.Domain.Interfaces;

namespace LogService.Application.Commands
{
    public class CreateLogCommand : IRequest<Guid>
    {
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
        public Dictionary<string, object>? Properties { get; set; }
    }

    public class CreateLogCommandHandler : IRequestHandler<CreateLogCommand, Guid>
    {
        private readonly ILogRepository _logRepository;

        public CreateLogCommandHandler(ILogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        public async Task<Guid> Handle(CreateLogCommand request, CancellationToken cancellationToken)
        {
            var logEntry = new LogEntry
            {
                Id = Guid.NewGuid(),
                ServiceName = request.ServiceName,
                Level = request.Level,
                Message = request.Message,
                Exception = request.Exception,
                StackTrace = request.StackTrace,
                UserId = request.UserId,
                IpAddress = request.IpAddress,
                Path = request.Path,
                Method = request.Method,
                StatusCode = request.StatusCode,
                Duration = request.Duration,
                Properties = request.Properties != null ?
                    System.Text.Json.JsonSerializer.Serialize(request.Properties) : null,
                Timestamp = DateTime.UtcNow
            };

            await _logRepository.AddAsync(logEntry);
            return logEntry.Id;
        }
    }
}