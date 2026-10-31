namespace Schemventory.Data;

public sealed class ProjectFilterSettings
{
    public bool ShowMissing { get; set; } = true;

    public bool ShowReplaced { get; set; } = true;

    public bool ShowCollected { get; set; }

    public bool ShowIgnored { get; set; }
}
