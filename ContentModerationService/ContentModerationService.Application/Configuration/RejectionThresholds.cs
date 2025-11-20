namespace ContentModerationService.Application.Configuration;

public class RejectionThresholds
{
    public int Hate { get; set; }
    public int SelfHarm { get; set; }
    public int Sexual { get; set; }
    public int Violence { get; set; }
}