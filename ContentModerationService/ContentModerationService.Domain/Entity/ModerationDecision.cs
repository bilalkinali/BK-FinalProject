using ContentModerationService.Domain.Enums;
using Action = ContentModerationService.Domain.Enums.Action;

namespace ContentModerationService.Domain.Entity;

public class ModerationDecision
{
    protected ModerationDecision()
    {
    }

    private ModerationDecision(string correlationId, string content, Action suggestedAction, int? hate, int? selfHarm, int? sexual, int? violence)
    {
        CorrelationId = correlationId;
        Content = content;
        SuggestedAction = suggestedAction;
        Hate = hate;
        SelfHarm = selfHarm;
        Sexual = sexual;
        Violence = violence;
        CreatedAt = DateTime.UtcNow;
    }

    public int Id { get; protected set; }
    public string CorrelationId { get; protected set; }
    public string Content { get; protected set; }
    public Action SuggestedAction { get; protected set; }
    public int? Hate { get; protected set; }
    public int? SelfHarm { get; protected set; }
    public int? Sexual { get; protected set; }
    public int? Violence { get; protected set; }
    public DateTime CreatedAt { get; protected set; }

    public static ModerationDecision Create(string correlationId, string content, Action suggestedAction, Dictionary<Category, int?> severities)
    {
        return new ModerationDecision(
            correlationId,
            content, 
            suggestedAction, 
            severities.GetValueOrDefault(Category.Hate),
            severities.GetValueOrDefault(Category.SelfHarm),
            severities.GetValueOrDefault(Category.Sexual),
            severities.GetValueOrDefault(Category.Violence));
    }
}