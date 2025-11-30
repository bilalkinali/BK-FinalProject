namespace SalesforceService.Api.Auth;

public interface ISalesforceAuthService
{
    Task<(string accessToken, string instanceUrl)> GetSessionAsync();
}