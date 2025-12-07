namespace SalesforceService.Infrastructure.Auth;

public interface ISalesforceAuthService
{
    Task<(string accessToken, string instanceUrl)> GetSessionAsync();
}