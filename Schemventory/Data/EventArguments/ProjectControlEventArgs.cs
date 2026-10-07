namespace Schemventory.Data.EventArguments;

public class ProjectControlEventArgs : EventArgs
{
    public Project Project { get; }

    public ProjectControlEventArgs(Project project)
    {
        Project = project;
    }
}