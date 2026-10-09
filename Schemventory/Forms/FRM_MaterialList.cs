using Serilog;
using Schemventory.App;
using Schemventory.Data;
using Schemventory.Services;

namespace Schemventory.Forms;

public partial class FRM_MaterialList : Form
{
    private const string LogContext = "(FRM_MaterialList)";
    private const string WindowName = $"Material list - {Constants.AppName}";

    private readonly Project _project;
    private readonly ProjectMaterialService _projectMaterialService;
    private readonly ItemIconService _itemIconService;
    private readonly ItemDataProvider _itemDataProvider;

    private IReadOnlyCollection<ProjectMaterial> _materials = [];
    private List<ProjectMaterial> _viewMaterials = [];

    public FRM_MaterialList(DatabaseService databaseService, Project project, ItemIconService itemIconService, ItemDataProvider itemDataProvider)
    {
        InitializeComponent();

        Text = WindowName;

        _project = project;
        _projectMaterialService = new ProjectMaterialService(databaseService);
        _itemIconService = itemIconService;
        _itemDataProvider = itemDataProvider;

        PNL_MaterialList.Initialize(_itemIconService, _itemDataProvider, _projectMaterialService);

        PNL_MaterialList.MaterialStateChanged += MaterialControl_StateChanged;
        PNL_MaterialList.ScrollMetricsChanged += PNL_MaterialList_ScrollMetricsChanged;
        PNL_MaterialList.ScrollPositionChanged += PNL_MaterialList_ScrollPositionChanged;
        PNL_MaterialList.MouseWheelScrollRequested += PNL_MaterialList_MouseWheelScrollRequested;

        VSB_Main.Scroll += VSB_Main_Scroll;
    }

    private async void FRM_MaterialList_Load(object sender, EventArgs e)
    {
        Log.Debug("{LogContext} Material list window opened. ProjectId={ProjectId}, ProjectName={ProjectName}", LogContext, _project.Id, _project.Name);

        SuspendLayout();

        try
        {
            LoadMaterialList();
            FillProjectData();
            ApplyMaterialView(resetScrollPosition: true);

            await Task.Yield();

            Log.Debug("{LogContext} Material list ready. ProjectId={ProjectId}, MaterialCount={MaterialCount}", LogContext, _project.Id, _materials.Count);
        }
        catch(Exception exception)
        {
            Log.Error(exception, "{LogContext} Failed to initialise material list. ProjectId={ProjectId}", LogContext, _project.Id);
            throw;
        }
        finally
        {
            ResumeLayout(true);
            Opacity = 1;
        }
    }

    private void MaterialControl_StateChanged(object? sender, EventArgs e)
    {
        Log.Debug("{LogContext} Material state changed. ProjectId={ProjectId}", LogContext, _project.Id);

        ApplyMaterialView(resetScrollPosition: false);
        UpdateStatistics();
    }

    private void FillProjectData()
    {
        LBL_ProjectName.Text = _project.Name;
        UpdateStatistics();
    }

    private void UpdateStatistics()
    {
        Dictionary<string, (long RequiredAmount, long CollectedAmount)> groupedMaterials = new(StringComparer.Ordinal);

        foreach(ProjectMaterial material in _materials)
        {
            if(material.State == ProjectMaterialState.Ignored)
                continue;

            var itemId = NormalizeItemId(material.DisplayItemId);
            var collectedAmount = Math.Min(material.CollectedAmount, material.RequiredAmount);

            if(groupedMaterials.TryGetValue(itemId, out (long RequiredAmount, long CollectedAmount) current))
                groupedMaterials[itemId] = (current.RequiredAmount + material.RequiredAmount, current.CollectedAmount + collectedAmount);
            else
                groupedMaterials[itemId] = (material.RequiredAmount, collectedAmount);
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

        LBL_TotalBlocks.Text = $"{collectedBlocks}/{totalBlocks} Blocks collected";
        LBL_DifferentMaterialsCount.Text = $"{collectedMaterials}/{groupedMaterials.Count} Materials collected";
    }

    private static string NormalizeItemId(string itemId)
    {
        const string prefix = "minecraft:";

        return itemId.StartsWith(prefix, StringComparison.Ordinal) ? itemId[prefix.Length..] : itemId;
    }

    private void LoadMaterialList()
    {
        _materials = _projectMaterialService.GetByProjectId(_project.Id);
        Log.Debug("{LogContext} Project materials loaded. ProjectId={ProjectId}, Count={Count}", LogContext, _project.Id, _materials.Count);
    }

    private void ApplyMaterialView(bool resetScrollPosition)
    {
        _viewMaterials = _materials.Where(ShouldShowMaterial).OrderBy(x => GetStateSortOrder(x.State)).ThenByDescending(x => x.RequiredAmount).ToList();

        var hasVisibleMaterials = _viewMaterials.Count > 0;

        PNL_MaterialList.Visible = hasVisibleMaterials;
        LBL_FilterWarn.Visible = !hasVisibleMaterials;

        PNL_MaterialList.SetMaterials(_viewMaterials, resetScrollPosition);
        UpdateScrollBar();

        Log.Debug("{LogContext} Material view applied. ProjectId={ProjectId}, VisibleCount={VisibleCount}, ResetScroll={ResetScroll}", LogContext, _project.Id, _viewMaterials.Count, resetScrollPosition);
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

    private void UpdateScrollBar()
    {
        var totalRows = PNL_MaterialList.TotalRows;
        var viewportRows = Math.Max(1, PNL_MaterialList.ViewportRows);

        VSB_Main.Minimum = 0;
        VSB_Main.Maximum = Math.Max(0, totalRows - 1);
        VSB_Main.LargeChange = viewportRows;
        VSB_Main.SmallChange = 1;

        var maximumValue = PNL_MaterialList.MaximumScrollRow;
        var scrollValue = Math.Clamp(PNL_MaterialList.ScrollRow, 0, maximumValue);

        if(VSB_Main.Value != scrollValue)
            VSB_Main.Value = scrollValue;

        VSB_Main.Visible = PNL_MaterialList.Visible && maximumValue > 0;
    }

    private void VSB_Main_Scroll(object? sender, ScrollEventArgs e)
    {
        PNL_MaterialList.SetScrollRow(e.NewValue);
    }

    private void PNL_MaterialList_MouseWheelScrollRequested(int rowDelta)
    {
        var newValue = Math.Clamp(VSB_Main.Value + rowDelta, VSB_Main.Minimum, PNL_MaterialList.MaximumScrollRow);

        if(newValue == VSB_Main.Value)
            return;

        VSB_Main.Value = newValue;
        PNL_MaterialList.SetScrollRow(newValue);
    }

    private void PNL_MaterialList_ScrollMetricsChanged(object? sender, EventArgs e)
    {
        UpdateScrollBar();
    }

    private void PNL_MaterialList_ScrollPositionChanged(object? sender, EventArgs e)
    {
        var scrollValue = Math.Clamp(PNL_MaterialList.ScrollRow, 0, PNL_MaterialList.MaximumScrollRow);

        if(VSB_Main.Value != scrollValue)
            VSB_Main.Value = scrollValue;
    }

    private void CHB_ShowMissing_CheckedChanged(object sender, EventArgs e)
    {
        Log.Debug("{LogContext} Missing filter changed. Checked={Checked}", LogContext, CHB_ShowMissing.Checked);
        ApplyMaterialView(resetScrollPosition: true);
    }

    private void CBX_ShowReplaced_CheckedChanged(object sender, EventArgs e)
    {
        Log.Debug("{LogContext} Replaced filter changed. Checked={Checked}", LogContext, CBX_ShowReplaced.Checked);
        ApplyMaterialView(resetScrollPosition: true);
    }

    private void CBX_ShowCollected_CheckedChanged(object sender, EventArgs e)
    {
        Log.Debug("{LogContext} Collected filter changed. Checked={Checked}", LogContext, CBX_ShowCollected.Checked);
        ApplyMaterialView(resetScrollPosition: true);
    }

    private void CBX_ShowIgnored_CheckedChanged(object sender, EventArgs e)
    {
        Log.Debug("{LogContext} Ignored filter changed. Checked={Checked}", LogContext, CBX_ShowIgnored.Checked);
        ApplyMaterialView(resetScrollPosition: true);
    }

    private void IBN_About_Click(object sender, EventArgs e)
    {
        Log.Debug("{LogContext} About dialogue requested.", LogContext);

        using FRM_About aboutForm = new();

        _ = aboutForm.ShowDialog(this);
    }

    private void FRM_MaterialList_ResizeBegin(object sender, EventArgs e)
    {
        PNL_MaterialList.SuspendVirtualLayout();
    }

    private void FRM_MaterialList_ResizeEnd(object sender, EventArgs e)
    {
        PNL_MaterialList.ResumeVirtualLayout();
        UpdateScrollBar();
    }
}
