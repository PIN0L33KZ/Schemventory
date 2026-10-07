using Schemventory.App;
using Schemventory.Data;
using Schemventory.Services;

namespace Schemventory.Forms;

public partial class FRM_NewProject : Form
{
    private readonly ProjectPersistenceService _projectPersistenceService;
    private readonly MaterialImportService _materialImportService;
    private readonly Project? _currentProject;

    private bool IsEditMode => _currentProject is not null;

    private string WindowName => IsEditMode
        ? $"Update project - {Constants.AppName}"
        : $"New project - {Constants.AppName}";

    public FRM_NewProject(DatabaseService databaseService, Project? project = null)
    {
        InitializeComponent();

        _projectPersistenceService = new ProjectPersistenceService(databaseService);
        _materialImportService = new MaterialImportService();

        _currentProject = project;
    }

    private void FRM_NewProject_Load(object sender, EventArgs e)
    {
        ConfigureForm();

        if(IsEditMode)
            LoadProjectData();
    }

    private void ConfigureForm()
    {
        Text = WindowName;
        BTN_CreateProject.Text = IsEditMode
            ? "Update project"
            : "Create project";
    }

    private void LoadProjectData()
    {
        if(_currentProject is null)
            return;

        TBX_ProjectName.Text = _currentProject.Name;
        TBX_SchematicFilePath.Text = _currentProject.SchematicPath;
    }

    private void BTN_CreateProject_Click(object sender, EventArgs e)
    {
        if(!ValidateInput())
            return;

        if(IsEditMode)
            UpdateProject();
        else
            CreateProject();

        DialogResult = DialogResult.OK;
        Close();
    }

    private void CreateProject()
    {
        Project project = new()
        {
            Id = Guid.NewGuid(),
            Name = TBX_ProjectName.Text.Trim(),
            SchematicPath = TBX_SchematicFilePath.Text.Trim(),
            CreatedAtUtc = DateTime.UtcNow,
            LastOpenedAtUtc = DateTime.UtcNow
        };

        IReadOnlyCollection<MaterialEntry> materials = _materialImportService.ReadMaterialList(project.SchematicPath);

        _projectPersistenceService.Create(project, materials);
    }

    private void UpdateProject()
    {
        if(_currentProject is null)
            return;

        var newName = TBX_ProjectName.Text.Trim();
        var newSchematicPath = TBX_SchematicFilePath.Text.Trim();

        var schematicChanged = !string.Equals(
            _currentProject.SchematicPath,
            newSchematicPath,
            StringComparison.OrdinalIgnoreCase);

        IReadOnlyCollection<MaterialEntry>? materials = null;

        if(schematicChanged)
            materials = _materialImportService.ReadMaterialList(newSchematicPath);

        _currentProject.Name = newName;
        _currentProject.SchematicPath = newSchematicPath;

        _projectPersistenceService.Update(_currentProject, materials);
    }

    private void BTN_SelectSchematicFilePath_Click(object sender, EventArgs e)
    {
        using OpenFileDialog openFileDialog = GetSchematicOpenFileDialog();

        if(openFileDialog.ShowDialog() != DialogResult.OK)
            return;

        TBX_SchematicFilePath.Text = openFileDialog.FileName;
        TBX_SchematicFilePath.SelectionStart = 0;
        TBX_SchematicFilePath.SelectionLength = TBX_SchematicFilePath.TextLength;
        TBX_SchematicFilePath.ScrollToCaret();
    }

    private static OpenFileDialog GetSchematicOpenFileDialog()
    {
        return new OpenFileDialog
        {
            Title = "Select schematic file",
            Filter =
                "Litematic files (*.litematic)|*.litematic|" +
                "Sponge schematic files (*.schem)|*.schem",
            CheckFileExists = true,
            CheckPathExists = true,
            Multiselect = false
        };
    }

    private bool ValidateInput()
    {
        var projectName = TBX_ProjectName.Text.Trim();
        var schematicFilePath = TBX_SchematicFilePath.Text.Trim();

        if(string.IsNullOrWhiteSpace(projectName))
        {
            _ = MessageBox.Show(
                "Project name cannot be empty.",
                WindowName,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            return false;
        }

        if(string.IsNullOrWhiteSpace(schematicFilePath))
        {
            _ = MessageBox.Show(
                "Schematic file path cannot be empty.",
                WindowName,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            return false;
        }

        if(!File.Exists(schematicFilePath))
        {
            _ = MessageBox.Show(
                "Schematic file does not exist.",
                WindowName,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            return false;
        }

        var extension = Path.GetExtension(schematicFilePath);

        if(!extension.Equals(".litematic", StringComparison.OrdinalIgnoreCase) &&
           !extension.Equals(".schem", StringComparison.OrdinalIgnoreCase))
        {
            _ = MessageBox.Show(
                "The selected file format is not supported.",
                WindowName,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            return false;
        }

        return true;
    }

    private void BTN_Cancel_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }
}