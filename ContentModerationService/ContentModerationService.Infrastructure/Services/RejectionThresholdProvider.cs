using ContentModerationService.Application.Configuration;
using ContentModerationService.Domain.Enums;
using Microsoft.Extensions.Options;

namespace ContentModerationService.Infrastructure.Services;

public class RejectionThresholdProvider : IRejectionThresholdProvider
{
    private readonly RejectionThresholds _thresholds;

    public RejectionThresholdProvider(IOptions<RejectionThresholds> thresholds)
    {
        _thresholds = thresholds.Value;
    }

    Dictionary<Category, int> IRejectionThresholdProvider.GetRejectionThresholds()
    {
        return new Dictionary<Category, int>
        {
            { Category.Hate, _thresholds.Hate },
            { Category.SelfHarm, _thresholds.SelfHarm },
            { Category.Sexual, _thresholds.Sexual },
            { Category.Violence, _thresholds.Violence }
        };
    }
}