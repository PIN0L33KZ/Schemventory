public sealed class ProjectMaterial
{
    public Guid ProjectId { get; init; }

    public required string ItemId { get; init; }

    public long RequiredAmount { get; set; }

    public long CollectedAmount { get; set; }

    public int MaxStackSize { get; set; } = 64;

    public ProjectMaterialState State { get; set; } = ProjectMaterialState.Missing;

    public string? ReplacementItemId { get; set; }

    public string DisplayItemId => !string.IsNullOrWhiteSpace(ReplacementItemId)
        ? ReplacementItemId
        : ItemId;

    public long RemainingAmount => State == ProjectMaterialState.Collected
        ? 0
        : Math.Max(0, RequiredAmount - CollectedAmount);

    public bool IsCompleted => State == ProjectMaterialState.Collected;

    public long Stacks => RequiredAmount / MaxStackSize;

    public long Blocks => RequiredAmount % MaxStackSize;
}
