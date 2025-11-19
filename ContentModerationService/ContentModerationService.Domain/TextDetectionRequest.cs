namespace ContentModerationService.Domain;

public class TextDetectionRequest(string text) : DetectionRequest
{
    public string Text { get; set; } = text;
}