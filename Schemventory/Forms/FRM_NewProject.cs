using Serilog;
using Schemventory.App;
using Schemventory.Data;
using Schemventory.Services;

namespace Schemventory.Forms;

public partial class FRM_NewProject : Form
{
    private const string LogContext = "(FRM_NewProject)";
    private readonly ProjectPersistenceService _projectPersistenceService;
    private readonly MaterialImportService _materialImportService;
    private readonly Project? _currentProject;

    private bool IsEditMode => _currentProject is not null;

    private string WindowName => IsEditMode ? $"Update project - {Constants.AppName}" : $"New project - {Constants.AppName}";

    public FRM_NewProject(DatabaseService databaseService, Project? project = null)
    {
        InitializeComponent();

        _projectPersistenceService = new ProjectPersistenceService(databaseService);
        _materialImportService = new MaterialImportService();
        _currentProject = project;
    }

    private void FRM_NewProject_Load(object sender, EventArgs e)
    {
        Log.Debug("{LogContext} Project dialogue opened. EditMode={EditMode}, ProjectId={ProjectId}", LogContext, IsEditMode, _currentProject?.Id);

        ConfigureForm();

        if(IsEditMode)
            LoadProjectData();
    }

    private void ConfigureForm()
    {
        Text = WindowName;
        BTN_CreateProject.Text = IsEditMode ? "Update project" : "Create project";
    }

    private void LoadProjectData()
    {
        if(_currentProject is null)
            return;

        TBX_ProjectName.Text = _currentProject.Name;
        TBX_SchematicFilePath.Text = _currentProject.SchematicPath;

        Log.Debug("{LogContext} Existing project data loaded. ProjectId={ProjectId}", LogContext, _currentProject.Id);
    }

    private void BTN_CreateProject_Click(object sender, EventArgs e)
    {
        Log.Debug("{LogContext} Project save requested. EditMode={EditMode}", LogContext, IsEditMode);

        if(!ValidateInput())
            return;

        try
        {
            if(IsEditMode)
                UpdateProject();
            else
                CreateProject();

            DialogResult = DialogResult.OK;
            Close();
        }
        catch(Exception exception)
        {
            Log.Error(exception, "{LogContext} Failed to save project. EditMode={EditMode}, ProjectId={ProjectId}", LogContext, IsEditMode, _currentProject?.Id);
            throw;
        }
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

        Log.Debug("{LogContext} Creating project. ProjectId={ProjectId}, Name={ProjectName}, SchematicPath={SchematicPath}", LogContext, project.Id, project.Name, project.SchematicPath);

        IReadOnlyCollection<MaterialEntry> materials = _materialImportService.ReadMaterialList(project.SchematicPath);

        _projectPersistenceService.Create(project, materials);

        Log.Information("{LogContext} Project created. ProjectId={ProjectId}, MaterialCount={MaterialCount}", LogContext, project.Id, materials.Count);
    }

    private void UpdateProject()
    {
        if(_currentProject is null)
            return;

        var newName = TBX_ProjectName.Text.Trim();
        var newSchematicPath = TBX_SchematicFilePath.Text.Trim();
        var schematicChanged = !string.Equals(_currentProject.SchematicPath, newSchematicPath, StringComparison.OrdinalIgnoreCase);

        Log.Debug("{LogContext} Updating project. ProjectId={ProjectId}, SchematicChanged={SchematicChanged}", LogContext, _currentProject.Id, schematicChanged);

        IReadOnlyCollection<MaterialEntry>? materials = null;

        if(schematicChanged)
            materials = _materialImportService.ReadMaterialList(newSchematicPath);

        _currentProject.Name = newName;
        _currentProject.SchematicPath = newSchematicPath;

        _projectPersistenceService.Update(_currentProject, materials);

        Log.Information("{LogContext} Project updated. ProjectId={ProjectId}", LogContext, _currentProject.Id);
    }

    private void BTN_SelectSchematicFilePath_Click(object sender, EventArgs e)
    {
        Log.Debug("{LogContext} Schematic file selection requested.", LogContext);

        using OpenFileDialog openFileDialog = GetSchematicOpenFileDialog();

        if(openFileDialog.ShowDialog() != DialogResult.OK)
        {
            Log.Debug("{LogContext} Schematic file selection cancelled.", LogContext);
            return;
        }

        TBX_SchematicFilePath.Text = openFileDialog.FileName;
        TBX_SchematicFilePath.SelectionStart = 0;
        TBX_SchematicFilePath.SelectionLength = TBX_SchematicFilePath.TextLength;
        TBX_SchematicFilePath.ScrollToCaret();

        Log.Debug("{LogContext} Schematic file selected. Path={Path}", LogContext, openFileDialog.FileName);
    }

    private static OpenFileDialog GetSchematicOpenFileDialog()
    {
        return new OpenFileDialog
        {
            Title = "Select schematic file",
            Filter = "Litematic files (*.litematic)|*.litematic|Sponge schematic files (*.schem)|*.schem",
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
            Log.Warning("{LogContext} Project validation failed: project name is empty.", LogContext);
            _ = MessageBox.Show("Project name cannot be empty.", WindowName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        if(string.IsNullOrWhiteSpace(schematicFilePath))
        {
            Log.Warning("{LogContext} Project validation failed: schematic file path is empty.", LogContext);
            _ = MessageBox.Show("Schematic file path cannot be empty.", WindowName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        if(!File.Exists(schematicFilePath))
        {
            Log.Warning("{LogContext} Project validation failed: schematic file does not exist. Path={Path}", LogContext, schematicFilePath);
            _ = MessageBox.Show("Schematic file does not exist.", WindowName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        var extension = Path.GetExtension(schematicFilePath);

        if(!extension.Equals(".litematic", StringComparison.OrdinalIgnoreCase) && !extension.Equals(".schem", StringComparison.OrdinalIgnoreCase))
        {
            Log.Warning("{LogContext} Project validation failed: unsupported schematic format. Extension={Extension}", LogContext, extension);
            _ = MessageBox.Show("The selected file format is not supported.", WindowName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        return true;
    }

    private void BTN_Cancel_Click(object sender, EventArgs e)
    {
        Log.Debug("{LogContext} Project dialogue cancelled. EditMode={EditMode}, ProjectId={ProjectId}", LogContext, IsEditMode, _currentProject?.Id);

        DialogResult = DialogResult.Cancel;
        Close();
    }
}
