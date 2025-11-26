using SalesforceService.Api.Auth;

namespace SalesforceService.Api;

public class SalesforceOutboundPublisher
{
    private readonly ILogger<SalesforceOutboundPublisher> _logger;
    private readonly ISalesforceAuthService _authService;

    public SalesforceOutboundPublisher(
        ILogger<SalesforceOutboundPublisher> logger,
        ISalesforceAuthService authService
        )
    {
        _logger = logger;
        _authService = authService;
    }
}