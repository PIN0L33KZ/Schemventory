using Schemventory.App;
using Schemventory.Data;
using Schemventory.Data.EventArguments;
using Schemventory.Forms;
using Schemventory.Services;

namespace Schemventory;

public partial class FRM_RecentProjects : Form
{
    private const string WindowName = $"Recent projects - {Constants.AppName}";

    private readonly DatabaseService _databaseService;
    private readonly ProjectService _projectService;

    public FRM_RecentProjects(DatabaseService databaseService)
    {
        InitializeComponent();

        Text = WindowName;
        LBL_AppTitle.Text = Constants.AppName;

        _databaseService = databaseService;
        _projectService = new ProjectService(databaseService);
    }

    private void FRM_RecentProjects_Load(object sender, EventArgs e)
    {
        LoadProjects();
    }

    private void BTN_CloseApp_Click(object sender, EventArgs e)
    {
        Application.Exit();
    }

    private void BTN_NewProject_Click(object sender, EventArgs e)
    {
        using FRM_NewProject newProjectForm = new(_databaseService);

        if(newProjectForm.ShowDialog() == DialogResult.OK)
            LoadProjects();
    }

    private void LoadProjects()
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
    }

    private List<ProjectControl> GetProjectControls()
    {
        List<ProjectControl> projectControls = [];
        IReadOnlyCollection<Project> projects = _projectService.GetAll();

        foreach(Project project in projects.OrderByDescending(x => x.LastOpenedAtUtc))
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
        Hide();

        FRM_MaterialList materialListForm = new(_databaseService, e.Project);
        _ = materialListForm.ShowDialog();

        Show();
    }

    private void ProjectControl_EditRequested(object? sender, ProjectControlEventArgs e)
    {
        using FRM_NewProject editProjectForm = new(_databaseService, e.Project);

        if(editProjectForm.ShowDialog() != DialogResult.OK)
            return;

        LoadProjects();
    }

    private void ProjectControl_DeleteRequested(object? sender, ProjectControlEventArgs e)
    {
        DialogResult result = MessageBox.Show(
            $"Do you really want to delete project '{e.Project.Name}'?",
            $"Delete project - {Constants.AppName}",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if(result != DialogResult.Yes)
            return;

        _projectService.Delete(e.Project.Id);
        LoadProjects();
    }
}