using Schemventory.App;
using Schemventory.Controls;
using Schemventory.Data;
using Schemventory.Services;

namespace Schemventory.Forms;

public partial class FRM_MaterialList : Form
{
    private const string WindowName = $"Material list - {Constants.AppName}";

    private readonly Project _project;
    private IReadOnlyCollection<ProjectMaterial> _materials = [];
    private readonly ProjectMaterialService _projectMaterialService;
    private readonly ItemIconService _itemIconService;
    private readonly ItemDataProvider _itemDataProvider;

    public FRM_MaterialList(DatabaseService databaseService, Project project)
    {
        InitializeComponent();

        Text = WindowName;

        _project = project;
        _projectMaterialService = new ProjectMaterialService(databaseService);
        _itemIconService = new ItemIconService();
        _itemDataProvider = new ItemDataProvider();
    }

    private async void FRM_MaterialList_Load(object sender, EventArgs e)
    {
        SuspendLayout();
        PNL_MaterialList.SuspendLayout();

        try
        {
            LoadMaterialList();
            FillProjectData();
            FillMaterialControls();
            ApplyMaterialView();

            await Task.Yield();
        }
        finally
        {
            PNL_MaterialList.ResumeLayout(true);
            ResumeLayout(true);

            Opacity = 1;
        }
    }

    private void MaterialControl_StateChanged(object? sender, EventArgs e)
    {
        ApplyMaterialView();
        UpdateStatistics();
    }

    private void FillProjectData()
    {
        LBL_ProjectName.Text = _project.Name;
        UpdateStatistics();
    }

    private void UpdateStatistics()
    {
        ProjectMaterial[] relevantMaterials = [.. _materials
        .Where(x => x.State != ProjectMaterialState.Ignored)];

        var groupedMaterials = relevantMaterials
            .GroupBy(
                x => NormalizeItemId(x.DisplayItemId),
                StringComparer.Ordinal)
            .Select(group => new
            {
                RequiredAmount = group.Sum(x => x.RequiredAmount),
                CollectedAmount = group.Sum(x =>
                    Math.Min(x.CollectedAmount, x.RequiredAmount))
            })
            .ToArray();

        var collectedBlocks = groupedMaterials
            .Sum(x => Math.Min(x.CollectedAmount, x.RequiredAmount));

        var totalBlocks = groupedMaterials
            .Sum(x => x.RequiredAmount);

        var collectedMaterials = groupedMaterials
            .Count(x => x.CollectedAmount >= x.RequiredAmount);

        var totalMaterials = groupedMaterials.Length;

        LBL_TotalBlocks.Text =
            $"{collectedBlocks}/{totalBlocks} Blocks collected";

        LBL_DifferentMaterialsCount.Text =
            $"{collectedMaterials}/{totalMaterials} Materials collected";
    }

    private static string NormalizeItemId(string itemId)
    {
        const string prefix = "minecraft:";

        return itemId.StartsWith(prefix, StringComparison.Ordinal)
            ? itemId[prefix.Length..]
            : itemId;
    }

    private void LoadMaterialList()
    {
        _materials = _projectMaterialService.GetByProjectId(_project.Id);
    }

    private void FillMaterialControls()
    {
        PNL_MaterialList.Controls.Clear();

        foreach(ProjectMaterial material in GetSortedMaterials())
        {
            MaterialControl materialControl = new(
                material,
                _itemIconService,
                _itemDataProvider,
                _projectMaterialService);

            materialControl.StateChanged += MaterialControl_StateChanged;

            PNL_MaterialList.Controls.Add(materialControl);
        }
    }

    private void ApplyMaterialView()
    {
        List<MaterialControl> controls = [.. PNL_MaterialList.Controls
            .OfType<MaterialControl>()];

        PNL_MaterialList.SuspendLayout();

        try
        {
            foreach(MaterialControl materialControl in controls)
                materialControl.Visible = ShouldShowMaterial(materialControl.Material);

            List<MaterialControl> sortedControls = [.. controls
                .OrderBy(x => GetStateSortOrder(x.Material.State))
                .ThenByDescending(x => x.Material.RequiredAmount)];

            for(var i = 0; i < sortedControls.Count; i++)
                PNL_MaterialList.Controls.SetChildIndex(sortedControls[i], i);
        }
        finally
        {
            PNL_MaterialList.ResumeLayout();
        }
    }

    private bool ShouldShowMaterial(ProjectMaterial material)
    {
        return material.State switch
        {
            ProjectMaterialState.Missing => CHB_ShowMissing.Checked,
            ProjectMaterialState.Replaced => CBX_ShowReplaced.Checked,
            ProjectMaterialState.Collected => CBX_ShowCollected.Checked,
            ProjectMaterialState.Ignored => CBX_ShowIgnored.Checked,
            _ => false
        };
    }

    private IEnumerable<ProjectMaterial> GetSortedMaterials()
    {
        return _materials
            .OrderBy(x => GetStateSortOrder(x.State))
            .ThenByDescending(x => x.RequiredAmount);
    }

    private static int GetStateSortOrder(ProjectMaterialState state)
    {
        return state switch
        {
            ProjectMaterialState.Missing or ProjectMaterialState.Replaced => 0,
            ProjectMaterialState.Collected => 1,
            ProjectMaterialState.Ignored => 2,
            _ => 3
        };
    }

    private void CHB_ShowMissing_CheckedChanged(object sender, EventArgs e)
    {
        ApplyMaterialView();
    }

    private void CBX_ShowReplaced_CheckedChanged(object sender, EventArgs e)
    {
        ApplyMaterialView();
    }

    private void CBX_ShowCollected_CheckedChanged(object sender, EventArgs e)
    {
        ApplyMaterialView();
    }

    private void CBX_ShowIgnored_CheckedChanged(object sender, EventArgs e)
    {
        ApplyMaterialView();
    }

    private void IBN_About_Click(object sender, EventArgs e)
    {
        FRM_About aboutForm = new();
        _ = aboutForm.ShowDialog();
    }
}