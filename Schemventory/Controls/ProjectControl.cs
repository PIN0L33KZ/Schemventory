using Serilog;
using Schemventory.Data;
using Schemventory.Data.EventArguments;

namespace Schemventory;

public partial class ProjectControl : UserControl
{
    private const string LogContext = "(ProjectControl)";
    private readonly Project _project;
    private readonly System.Windows.Forms.Timer _hoverTimer;
    private bool _isHovered;

    public event EventHandler<ProjectControlEventArgs>? ControlClicked;
    public event EventHandler<ProjectControlEventArgs>? EditRequested;
    public event EventHandler<ProjectControlEventArgs>? DeleteRequested;

    public ProjectControl(Project project)
    {
        InitializeComponent();

        _project = project;

        _hoverTimer = new System.Windows.Forms.Timer
        {
            Interval = 50
        };

        _hoverTimer.Tick += HoverTimer_Tick;
        Disposed += ProjectControl_Disposed;

        RegisterEvents(this);
    }

    private void ProjectControl_Load(object sender, EventArgs e)
    {
        LoadProjectData();
        Log.Debug("{LogContext} Project control initialised. ProjectId={ProjectId}, Name={ProjectName}", LogContext, _project.Id, _project.Name);
    }

    private void ControlClickedHandler(object? sender, EventArgs e)
    {
        Log.Debug("{LogContext} Project control clicked. ProjectId={ProjectId}", LogContext, _project.Id);
        ControlClicked?.Invoke(this, new ProjectControlEventArgs(_project));
    }

    private void HoverStart(object? sender, EventArgs e)
    {
        if(_isHovered)
            return;

        _isHovered = true;

        IBN_EditProject.Show();
        IBN_DeleteProject.Show();
        PNL_Background.BorderColor = Color.FromArgb(162, 123, 90);

        _hoverTimer.Start();
    }

    private void HoverTimer_Tick(object? sender, EventArgs e)
    {
        if(RectangleToScreen(ClientRectangle).Contains(Cursor.Position))
            return;

        EndHover();
    }

    private void EndHover()
    {
        _hoverTimer.Stop();
        _isHovered = false;

        IBN_EditProject.Hide();
        IBN_DeleteProject.Hide();
        PNL_Background.BorderColor = Color.FromArgb(103, 99, 99);
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

        if(control != this && control is not Guna.UI2.WinForms.Guna2ImageButton)
            control.Click += ControlClickedHandler;

        foreach(Control child in control.Controls)
            RegisterEvents(child);
    }

    private void IBN_EditProject_Click(object sender, EventArgs e)
    {
        Log.Debug("{LogContext} Project edit requested. ProjectId={ProjectId}", LogContext, _project.Id);
        EditRequested?.Invoke(this, new ProjectControlEventArgs(_project));
    }

    private void IBN_DeleteProject_Click(object sender, EventArgs e)
    {
        Log.Debug("{LogContext} Project deletion requested. ProjectId={ProjectId}", LogContext, _project.Id);
        DeleteRequested?.Invoke(this, new ProjectControlEventArgs(_project));
    }

    private void ProjectControl_Disposed(object? sender, EventArgs e)
    {
        _hoverTimer.Stop();
        _hoverTimer.Dispose();
    }

    public void RefreshProject()
    {
        LoadProjectData();
        Log.Debug("{LogContext} Project control refreshed. ProjectId={ProjectId}", LogContext, _project.Id);
    }
}
