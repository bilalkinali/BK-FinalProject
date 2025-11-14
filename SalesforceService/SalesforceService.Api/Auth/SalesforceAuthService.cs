using System.Xml.Linq;

namespace SalesforceService.Api.Auth;

public class SalesforceAuthService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private string? _sessionId;
    private string? _serverUrl;

    public SalesforceAuthService(IConfiguration configuration, HttpClient httpClient)
    {
        _configuration = configuration;
        _httpClient = httpClient;
    }

    public async Task<(string sessionId, string instanceUrl)> GetSessionAsync()
    {
        if (_sessionId != null && _serverUrl != null)
        {
            return (_sessionId, _serverUrl);
        }

        var username = _configuration["Salesforce:Username"];
        var password = _configuration["Salesforce:Password"];

        // SOAP login request for testing
        var soapEnvelope = $@"<?xml version=""1.0"" encoding=""utf-8""?>
            <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" 
                              xmlns:urn=""urn:enterprise.soap.sforce.com"">
                <soapenv:Body>
                    <urn:login>
                        <urn:username>{username}</urn:username>
                        <urn:password>{password}</urn:password>
                    </urn:login>
                </soapenv:Body>
            </soapenv:Envelope>";

        var content = new StringContent(soapEnvelope, System.Text.Encoding.UTF8, "text/xml");
        content.Headers.Add("SOAPAction", "login");

        var response = await _httpClient.PostAsync(
            "https://login.salesforce.com/services/Soap/c/60.0",
            content
        );

        response.EnsureSuccessStatusCode();

        var responseXml = await response.Content.ReadAsStringAsync();
        var doc = XDocument.Parse(responseXml);

        var ns = XNamespace.Get("urn:enterprise.soap.sforce.com");
        var sessionId = doc.Descendants(ns + "sessionId").First().Value;
        var serverUrl = doc.Descendants(ns + "serverUrl").First().Value;

        // Extract instance URL from serverUrl
        var uri = new Uri(serverUrl);
        var instanceUrl = $"{uri.Scheme}://{uri.Host}";

        _sessionId = sessionId;
        _serverUrl = instanceUrl;

        return (_sessionId, _serverUrl);
    }
}