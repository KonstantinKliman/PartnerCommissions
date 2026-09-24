namespace Users.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string ExternalId { get; set; } = null!;
    public Guid? PartnerId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}