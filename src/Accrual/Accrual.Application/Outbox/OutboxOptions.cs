using System.ComponentModel.DataAnnotations;

namespace Accrual.Application.Outbox;

public sealed class OutboxOptions
{
    public const string SectionName = "Outbox";
    
    [Range(typeof(TimeSpan), "00:00:01", "00:10:00")]
    public TimeSpan PollingInterval { get; set; }
    
    [Range(1, 1000)]
    public int BatchSize { get; set; }
    
    [Range(typeof(TimeSpan), "00:00:01", "01:00:00")]
    public TimeSpan MaxRetryDelay { get; set; }
}