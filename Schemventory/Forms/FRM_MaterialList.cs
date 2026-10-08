using Schemventory.App;
using Schemventory.Controls;
using Schemventory.Data;
using Schemventory.Services;

namespace Schemventory.Forms;

public partial class FRM_MaterialList : Form
{
    private const string WindowName = $"Material list - {Constants.AppName}";

    private readonly Project _project;
    private readonly ProjectMaterialService _projectMaterialService;
    private readonly ItemIconService _itemIconService;
    private readonly ItemDataProvider _itemDataProvider;
    private readonly List<MaterialControl> _materialControls = [];

    private IReadOnlyCollection<ProjectMaterial> _materials = [];

    public FRM_MaterialList(
        DatabaseService databaseService,
        Project project,
        ItemIconService itemIconService,
        ItemDataProvider itemDataProvider)
    {
        InitializeComponent();

        Text = WindowName;

        _project = project;
        _projectMaterialService = new ProjectMaterialService(databaseService);
        _itemIconService = itemIconService;
        _itemDataProvider = itemDataProvider;
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
            ApplyMaterialFilter();

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
        SortMaterialControls();
        ApplyMaterialFilter();
        UpdateStatistics();
    }

    private void FillProjectData()
    {
        LBL_ProjectName.Text = _project.Name;
        UpdateStatistics();
    }

    private void UpdateStatistics()
    {
        Dictionary<string, (long RequiredAmount, long CollectedAmount)> groupedMaterials =
            new(StringComparer.Ordinal);

        foreach(ProjectMaterial material in _materials)
        {
            if(material.State == ProjectMaterialState.Ignored)
                continue;

            var itemId = NormalizeItemId(material.DisplayItemId);
            var collectedAmount = Math.Min(material.CollectedAmount, material.RequiredAmount);

            if(groupedMaterials.TryGetValue(itemId, out (long RequiredAmount, long CollectedAmount) current))
            {
                groupedMaterials[itemId] = (
                    current.RequiredAmount + material.RequiredAmount,
                    current.CollectedAmount + collectedAmount);
            }
            else
            {
                groupedMaterials[itemId] = (
                    material.RequiredAmount,
                    collectedAmount);
            }
        }

        long collectedBlocks = 0;
        long totalBlocks = 0;
        var collectedMaterials = 0;

        foreach((var RequiredAmount, var CollectedAmount) in groupedMaterials.Values)
        {
            totalBlocks += RequiredAmount;
            collectedBlocks += Math.Min(CollectedAmount, RequiredAmount);

            if(CollectedAmount >= RequiredAmount)
                collectedMaterials++;
        }

        LBL_TotalBlocks.Text =
            $"{collectedBlocks}/{totalBlocks} Blocks collected";

        LBL_DifferentMaterialsCount.Text =
            $"{collectedMaterials}/{groupedMaterials.Count} Materials collected";
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
        _materialControls.Clear();

        foreach(ProjectMaterial material in GetSortedMaterials())
        {
            MaterialControl materialControl = new(
                material,
                _itemIconService,
                _itemDataProvider,
                _projectMaterialService);

            materialControl.StateChanged += MaterialControl_StateChanged;

            _materialControls.Add(materialControl);
        }

        if(_materialControls.Count > 0)
            PNL_MaterialList.Controls.AddRange([.. _materialControls]);
    }

    private void ApplyMaterialFilter()
    {
        var hasVisibleControls = false;

        PNL_MaterialList.SuspendLayout();

        try
        {
            foreach(MaterialControl materialControl in _materialControls)
            {
                var isVisible = ShouldShowMaterial(materialControl.Material);

                if(materialControl.Visible != isVisible)
                    materialControl.Visible = isVisible;

                if(isVisible)
                    hasVisibleControls = true;
            }

            if(hasVisibleControls)
            {
                if(!PNL_MaterialList.Visible)
                    PNL_MaterialList.Show();

                if(LBL_FilterWarn.Visible)
                    LBL_FilterWarn.Hide();
            }
            else
            {
                if(PNL_MaterialList.Visible)
                    PNL_MaterialList.Hide();

                if(!LBL_FilterWarn.Visible)
                    LBL_FilterWarn.Show();
            }
        }
        finally
        {
            PNL_MaterialList.ResumeLayout();
        }
    }

    private void SortMaterialControls()
    {
        _materialControls.Sort(static (left, right) =>
        {
            var stateComparison = GetStateSortOrder(left.Material.State)
                .CompareTo(GetStateSortOrder(right.Material.State));

            return stateComparison != 0
                ? stateComparison
                : right.Material.RequiredAmount.CompareTo(left.Material.RequiredAmount);
        });

        PNL_MaterialList.SuspendLayout();

        try
        {
            for(var i = 0; i < _materialControls.Count; i++)
                PNL_MaterialList.Controls.SetChildIndex(_materialControls[i], i);
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
        ApplyMaterialFilter();
    }

    private void CBX_ShowReplaced_CheckedChanged(object sender, EventArgs e)
    {
        ApplyMaterialFilter();
    }

    private void CBX_ShowCollected_CheckedChanged(object sender, EventArgs e)
    {
        ApplyMaterialFilter();
    }

    private void CBX_ShowIgnored_CheckedChanged(object sender, EventArgs e)
    {
        ApplyMaterialFilter();
    }

    private void IBN_About_Click(object sender, EventArgs e)
    {
        FRM_About aboutForm = new();
        _ = aboutForm.ShowDialog();
    }

    private void FRM_MaterialList_ResizeBegin(object sender, EventArgs e)
    {
        PNL_MaterialList.SuspendLayout();
    }

    private void FRM_MaterialList_ResizeEnd(object sender, EventArgs e)
    {
        PNL_MaterialList.ResumeLayout(true);
    }
}
