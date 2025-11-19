using ContentModerationService.Domain.Enums;

namespace ContentModerationService.Application.Configuration
{
    public interface IRejectionThresholdProvider
    {
        Dictionary<Category, int> GetRejectionThresholds();
    }
}