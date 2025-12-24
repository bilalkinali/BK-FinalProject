namespace ContentModerationService.Application.Queries.QueryDto;

public record ModerationResultDto
(
    int ModerationId,
    string CorrelationId,
    string Content, 
    string SuggestedAction,
    int Hate, 
    int SelfHarm,
    int Sexual, 
    int Violence, 
    string CreatedAt
);