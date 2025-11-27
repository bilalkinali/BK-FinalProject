using System.Text.Json;

namespace SalesforceService.Api.Auth;

public class SalesforceAuthService : ISalesforceAuthService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly ILogger<SalesforceAuthService> _logger;
    private readonly SalesforceTokenCache _cache;

    public SalesforceAuthService(
        IConfiguration configuration,
        HttpClient httpClient,
        ILogger<SalesforceAuthService> logger,
        SalesforceTokenCache cache)
    {
        _configuration = configuration;
        _httpClient = httpClient;
        _logger = logger;
        _cache = cache;
    }

    async Task<(string accessToken, string instanceUrl)> ISalesforceAuthService.GetSessionAsync()
    {
        if (!string.IsNullOrEmpty(_cache.AccessToken) && !string.IsNullOrEmpty(_cache.InstanceUrl))
        {
            return (_cache.AccessToken, _cache.InstanceUrl);
        }

        var loginUrl = _configuration["Salesforce:LoginUrl"]!;
        var clientId = _configuration["Salesforce:ClientId"]!;
        var clientSecret = _configuration["Salesforce:ClientSecret"]!;

        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" },
            { "client_id", clientId },
            { "client_secret", clientSecret }
        });

        _logger.LogInformation("Sending OAuth login request to Salesforce...");

        var response = await _httpClient.PostAsync(loginUrl, content);
        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Salesforce OAuth Error Response: {Json}", json);
            response.EnsureSuccessStatusCode();
        }

        using var doc = JsonDocument.Parse(json);
        _cache.AccessToken = doc.RootElement.GetProperty("access_token").GetString();
        _cache.InstanceUrl = doc.RootElement.GetProperty("instance_url").GetString();

        _logger.LogInformation("Salesforce OAuth login successful. Instance={Instance}", _cache.InstanceUrl);

        return (_cache.AccessToken!, _cache.InstanceUrl!);
    }
}