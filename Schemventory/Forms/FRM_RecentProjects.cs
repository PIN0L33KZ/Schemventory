using Serilog;
using Schemventory.App;
using Schemventory.Data;
using Schemventory.Data.EventArguments;
using Schemventory.Forms;
using Schemventory.Services;

namespace Schemventory;

public partial class FRM_RecentProjects : Form
{
    private const string LogContext = "(FRM_RecentProjects)";
    private const string WindowName = $"Recent projects - {Constants.AppName}";

    private readonly DatabaseService _databaseService;
    private readonly ProjectService _projectService;
    private readonly ItemIconService _itemIconService;
    private readonly ItemDataProvider _itemDataProvider;

    public FRM_RecentProjects(DatabaseService databaseService, ItemIconService itemIconService, ItemDataProvider itemDataProvider)
    {
        InitializeComponent();

        Text = WindowName;
        LBL_AppTitle.Text = Constants.AppName;

        _databaseService = databaseService;
        _projectService = new ProjectService(databaseService);
        _itemIconService = itemIconService;
        _itemDataProvider = itemDataProvider;
    }

    private void FRM_RecentProjects_Load(object sender, EventArgs e)
    {
        Log.Debug("{LogContext} Recent projects window opened.", LogContext);
        LoadProjects();
    }

    private void BTN_CloseApp_Click(object sender, EventArgs e)
    {
        Log.Debug("{LogContext} Application close requested.", LogContext);
        Application.Exit();
    }

    private void BTN_NewProject_Click(object sender, EventArgs e)
    {
        Log.Debug("{LogContext} New project dialogue requested.", LogContext);

        using FRM_NewProject newProjectForm = new(_databaseService);

        if(newProjectForm.ShowDialog() == DialogResult.OK)
        {
            Log.Debug("{LogContext} New project dialogue completed successfully.", LogContext);
            LoadProjects();
        }
        else
        {
            Log.Debug("{LogContext} New project dialogue cancelled.", LogContext);
        }
    }

    private void LoadProjects()
    {
        Log.Debug("{LogContext} Loading recent projects.", LogContext);

        PNL_RecentProjectControls.SuspendLayout();

        try
        {
            PNL_RecentProjectControls.Controls.Clear();
            List<ProjectControl> projectControls = GetProjectControls();

            if(projectControls.Count > 0)
            {
                PNL_RecentProjectControls.Show();
                LBL_NoProjectsWarn.Hide();
                PNL_RecentProjectControls.Controls.AddRange([.. projectControls]);
            }
            else
            {
                PNL_RecentProjectControls.Hide();
                LBL_NoProjectsWarn.Show();
            }

            Log.Debug("{LogContext} Recent projects loaded. Count={Count}", LogContext, projectControls.Count);
        }
        catch(Exception exception)
        {
            Log.Error(exception, "{LogContext} Failed to load recent projects.", LogContext);
            throw;
        }
        finally
        {
            PNL_RecentProjectControls.ResumeLayout();
        }
    }

    private List<ProjectControl> GetProjectControls()
    {
        List<ProjectControl> projectControls = [];
        IReadOnlyCollection<Project> projects = _projectService.GetAll();

        foreach(Project project in projects)
        {
            ProjectControl projectControl = new(project)
            {
                Width = PNL_RecentProjectControls.Width - 25
            };

            projectControl.ControlClicked += ProjectControl_ControlClicked;
            projectControl.EditRequested += ProjectControl_EditRequested;
            projectControl.DeleteRequested += ProjectControl_DeleteRequested;

            projectControls.Add(projectControl);
        }

        return projectControls;
    }

    private void ProjectControl_ControlClicked(object? sender, ProjectControlEventArgs e)
    {
        Log.Debug("{LogContext} Project opened. ProjectId={ProjectId}, Name={ProjectName}", LogContext, e.Project.Id, e.Project.Name);

        Hide();

        using FRM_MaterialList materialListForm = new(_databaseService, e.Project, _itemIconService, _itemDataProvider);

        _ = materialListForm.ShowDialog();

        Show();

        Log.Debug("{LogContext} Returned from material list. ProjectId={ProjectId}", LogContext, e.Project.Id);
    }

    private void ProjectControl_EditRequested(object? sender, ProjectControlEventArgs e)
    {
        Log.Debug("{LogContext} Project edit requested. ProjectId={ProjectId}, Name={ProjectName}", LogContext, e.Project.Id, e.Project.Name);

        using FRM_NewProject editProjectForm = new(_databaseService, e.Project);

        if(editProjectForm.ShowDialog() != DialogResult.OK)
        {
            Log.Debug("{LogContext} Project edit cancelled. ProjectId={ProjectId}", LogContext, e.Project.Id);
            return;
        }

        Log.Information("{LogContext} Project edited. ProjectId={ProjectId}", LogContext, e.Project.Id);
        LoadProjects();
    }

    private void ProjectControl_DeleteRequested(object? sender, ProjectControlEventArgs e)
    {
        Log.Debug("{LogContext} Project deletion requested. ProjectId={ProjectId}, Name={ProjectName}", LogContext, e.Project.Id, e.Project.Name);

        DialogResult result = MessageBox.Show($"Do you really want to delete project '{e.Project.Name}'?", $"Delete project - {Constants.AppName}", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

        if(result != DialogResult.Yes)
        {
            Log.Debug("{LogContext} Project deletion cancelled. ProjectId={ProjectId}", LogContext, e.Project.Id);
            return;
        }

        try
        {
            _projectService.Delete(e.Project.Id);
            Log.Information("{LogContext} Project deleted. ProjectId={ProjectId}, Name={ProjectName}", LogContext, e.Project.Id, e.Project.Name);
            LoadProjects();
        }
        catch(Exception exception)
        {
            Log.Error(exception, "{LogContext} Failed to delete project. ProjectId={ProjectId}", LogContext, e.Project.Id);
            throw;
        }
    }
}
