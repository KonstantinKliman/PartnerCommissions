namespace Accrual.Application.Interfaces;

public interface IUsersClient
{
    Task<List<string>?> GetUplineAsync(string userExternalId, CancellationToken ct);
}