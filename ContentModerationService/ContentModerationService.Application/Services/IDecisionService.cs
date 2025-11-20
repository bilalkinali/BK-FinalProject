using ContentModerationService.Domain;
using ContentModerationService.Domain.Enums;

namespace ContentModerationService.Application.Services
{
    public interface IDecisionService
    {
        Decision MakeDecision(DetectionResult detectionResult, Dictionary<Category, int> rejectionThresholds);
    }
}