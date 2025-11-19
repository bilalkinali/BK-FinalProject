namespace ContentModerationService.Domain;

public class ImageDetectionRequest(string content) : DetectionRequest
{
    public Image Image { get; set; } = new Image(content);
}