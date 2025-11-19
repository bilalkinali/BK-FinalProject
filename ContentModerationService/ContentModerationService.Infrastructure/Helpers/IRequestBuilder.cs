using ContentModerationService.Domain.Enums;

namespace ContentModerationService.Infrastructure.Helpers;

public interface IRequestBuilder
{
    HttpRequestMessage BuildHttpRequestMessage(MediaType mediaType, string content);
}