namespace Schemventory.Data;

public sealed class Project
{
    public Guid Id { get; init; }

    public required string Name { get; set; }

    public required string SchematicPath { get; set; }

    public DateTime CreatedAtUtc { get; init; }

    public DateTime LastOpenedAtUtc { get; set; }
}