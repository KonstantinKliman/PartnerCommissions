using System.Text.Json;

namespace Accrual.Application.Outbox;

public class OutboxMessage
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    
    public string Type { get; set; } = null!;
    
    public string Payload { get; set; } = null!;
    
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    
    public DateTimeOffset NextAttemptAt { get; set; } = DateTimeOffset.UtcNow;
    
    public int Attempts { get; set; }
    
    public string? LastError { get; set; }
    
    public DateTimeOffset? ProcessedAt { get; set; }

    public static OutboxMessage Create<T>(string type, T payload) => new()
    {
        Type = type,
        Payload = JsonSerializer.Serialize(payload)
    };
}