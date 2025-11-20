using ContentModerationService.Domain.Enums;
using Action = ContentModerationService.Domain.Enums.Action;

namespace ContentModerationService.Domain;

public class Decision
{
    public Decision(Action suggestedAction, Dictionary<Category, Action> actionCategory)
    {
        ActionByCategory = actionCategory;
        SuggestedAction = suggestedAction;
    }
    public Action SuggestedAction { get; set; }
    public Dictionary<Category, Action> ActionByCategory { get; set; }
}