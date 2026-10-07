using Schemventory.Data;
using Schemventory.Data.EventArguments;

namespace Schemventory;

public partial class ProjectControl : UserControl
{
    public event EventHandler<ProjectControlEventArgs>? ControlClicked;
    public event EventHandler<ProjectControlEventArgs>? EditRequested;
    public event EventHandler<ProjectControlEventArgs>? DeleteRequested;

    private readonly Project _project;

    public ProjectControl(Project project)
    {
        InitializeComponent();

        _project = project;

        RegisterEvents(this);
    }

    private void ProjectControl_Load(object sender, EventArgs e)
    {
        LoadProjectData();
    }

    private void ControlClickedHandler(object? sender, EventArgs e)
    {
        ControlClicked?.Invoke(this, new ProjectControlEventArgs(_project));
    }

    private void HoverStart(object? sender, EventArgs e)
    {
        IBN_EditProject.Show();
        IBN_DeleteProject.Show();
        PNL_Background.BorderColor = Color.FromArgb(162, 123, 90);
    }

    private void HoverEnd(object? sender, EventArgs e)
    {
        _ = BeginInvoke(() =>
        {
            Point mousePosition = PointToClient(Cursor.Position);

            if(ClientRectangle.Contains(mousePosition))
                return;

            IBN_EditProject.Hide();
            IBN_DeleteProject.Hide();
            PNL_Background.BorderColor = Color.FromArgb(103, 99, 99);
        });
    }

    private void LoadProjectData()
    {
        LBL_ProjectName.Text = _project.Name;
        LBL_SchematicPath.Text = _project.SchematicPath;
        LBL_CreatedAt.Text = $"Created: {_project.CreatedAtUtc:dd.MM.yyyy HH:mm}";
        LBL_LastOpened.Text = $"Last opened: {_project.LastOpenedAtUtc:dd.MM.yyyy HH:mm}";

        switch(Path.GetExtension(_project.SchematicPath))
        {
            case ".litematic":
                PBX_ProjectIcon.Image = Properties.Resources.ProjectIconL;
                break;

            case ".schem":
                PBX_ProjectIcon.Image = Properties.Resources.ProjectIconS;
                break;
        }
    }

    private void RegisterEvents(Control control)
    {
        control.MouseEnter += HoverStart;
        control.MouseLeave += HoverEnd;

        if(control != this && control is not Guna.UI2.WinForms.Guna2ImageButton)
            control.Click += ControlClickedHandler;

        foreach(Control child in control.Controls)
            RegisterEvents(child);
    }

    private void IBN_EditProject_Click(object sender, EventArgs e)
    {
        EditRequested?.Invoke(this, new ProjectControlEventArgs(_project));
    }

    private void IBN_DeleteProject_Click(object sender, EventArgs e)
    {
        DeleteRequested?.Invoke(this, new ProjectControlEventArgs(_project));
    }

    public void RefreshProject()
    {
        LoadProjectData();
    }
}