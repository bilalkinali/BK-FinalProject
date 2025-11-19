using ContentModerationService.Domain;
using ContentModerationService.Domain.Enums;
using Action = ContentModerationService.Domain.Enums.Action;

namespace ContentModerationService.Application.Services;

public class DecisionService : IDecisionService
{
    public static readonly int[] VALID_THRESHOLD_VALUES = [-1, 0, 2, 4, 6];

    Decision IDecisionService.MakeDecision(DetectionResult detectionResult, Dictionary<Category, int> rejectionThresholds)
    {
        Dictionary<Category, Action> actionResult = new Dictionary<Category, Action>();
        Action finalAction = Action.Accept;

        foreach (KeyValuePair<Category, int> pair in rejectionThresholds)
        {
            if (!VALID_THRESHOLD_VALUES.Contains(pair.Value))
            {
                throw new ArgumentException($"RejectionThresholds can only be in ({string.Join(",",VALID_THRESHOLD_VALUES)})");
            }

            var severity = GetDetectionResultByCategory(pair.Key, detectionResult);
            if (severity is null)
            {
                throw new ArgumentException($"Cannot find detection result for {pair.Key}");
            }

            Action action;
            if (pair.Value != -1 && severity >= pair.Value)
                action = Action.Reject;
            else
                action = Action.Accept;

            actionResult[pair.Key] = action;

            if (action.CompareTo(finalAction) > 0) // Reject > Accept in enum order (1 > 0)
            {
                finalAction = action;
            }
            // Could add Review as third option, so it would go as Accept, Review, Reject, making Reject override the rest
        }

        Console.WriteLine(finalAction);
        foreach (var res in actionResult)
        {
            Console.WriteLine($"Category: {res.Key}, Action: {res.Value}");
        }

        return new Decision(finalAction, actionResult);
    }

    private int? GetDetectionResultByCategory(Category category, DetectionResult detectionResult)
    {
        int? severityResult = null;

        if (detectionResult.CategoryAnalysis != null)
        {
            foreach (var detailResult in detectionResult.CategoryAnalysis)
            {
                if (detailResult.Category == category.ToString())
                {
                    severityResult = detailResult.Severity ?? 0;
                }
            }
        }

        return severityResult;
    }
}