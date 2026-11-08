
using Schemventory.App;
using Schemventory.Controls;
using Schemventory.Data;
using Schemventory.Services;
using Serilog;

namespace Schemventory.Forms;

public partial class FRM_MaterialList : Form
{
    private const string LogContext = "(FRM_MaterialList)";

    private readonly Project _project;
    private readonly ProjectMaterialService _projectMaterialService;
    private readonly ProjectFilterSettingsService _projectFilterSettingsService;
    private readonly ItemIconService _itemIconService;
    private readonly ItemDataProvider _itemDataProvider;

    private IReadOnlyCollection<ProjectMaterial> _materials = [];
    private List<ProjectMaterial> _viewMaterials = [];
    private bool _loadingFilterSettings;
    private int? _pendingScrollOffset;
    private long _lastScrollInputTick;

    public FRM_MaterialList(DatabaseService databaseService, Project project, ItemIconService itemIconService, ItemDataProvider itemDataProvider)
    {
        InitializeComponent();

        _project = project;
        _projectMaterialService = new ProjectMaterialService(databaseService);
        _projectFilterSettingsService = new ProjectFilterSettingsService(databaseService);
        _itemIconService = itemIconService;
        _itemDataProvider = itemDataProvider;

        Text = $"Material list for {_project.Name} - {Constants.AppName}";

        PNL_MaterialList.Initialize(_itemIconService, _itemDataProvider, _projectMaterialService);

        PNL_MaterialList.ScrollMetricsChanged += PNL_MaterialList_ScrollMetricsChanged;
        PNL_MaterialList.ScrollPositionChanged += PNL_MaterialList_ScrollPositionChanged;
        PNL_MaterialList.MouseWheelScrollRequested += PNL_MaterialList_MouseWheelScrollRequested;
        PNL_MaterialList.MaterialStateChanged += MaterialControl_StateChanged;
        PNL_MaterialList.MaterialRemoveRequested += MaterialControl_RemoveRequested;

        FPN_Missing.CheckedChanged += FilterPanel_CheckedChanged;
        FPN_Replaced.CheckedChanged += FilterPanel_CheckedChanged;
        FPN_Collected.CheckedChanged += FilterPanel_CheckedChanged;
        FPN_Ignored.CheckedChanged += FilterPanel_CheckedChanged;
    }

    private async void FRM_MaterialList_Load(object sender, EventArgs e)
    {
        Log.Debug("{LogContext} Material list window opened. ProjectId={ProjectId}, ProjectName={ProjectName}", LogContext, _project.Id, _project.Name);

        SuspendLayout();

        try
        {
            LoadMaterialList();
            LoadFilterSettings();
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

    private void MaterialControl_RemoveRequested(ProjectMaterial material)
    {
        DialogResult result = MessageBox.Show($"Do you really want to remove '{ItemIdService.GetDisplayName(material.ItemId)}' from this project?\n\n" + "This action cannot be undone.", $"Remove material - {Constants.AppName}", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

        if(result != DialogResult.Yes)
        {
            Log.Debug("{LogContext} Material removal cancelled. ProjectId={ProjectId}, ItemId={ItemId}", LogContext, material.ProjectId, material.ItemId);

            return;
        }

        try
        {
            _projectMaterialService.Remove(material.ProjectId, material.ItemId);

            LoadMaterialList();
            ApplyMaterialView(resetScrollPosition: false);
            UpdateStatistics();

            Log.Information("{LogContext} Material removed. ProjectId={ProjectId}, ItemId={ItemId}", LogContext, material.ProjectId, material.ItemId);
        }
        catch(Exception exception)
        {
            Log.Error(exception, "{LogContext} Failed to remove material. ProjectId={ProjectId}, ItemId={ItemId}", LogContext, material.ProjectId, material.ItemId);

            _ = MessageBox.Show($"The material could not be removed.\n\n{exception.Message}", Constants.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void FillProjectData()
    {
        LBL_ProjectName.Text = _project.Name;
        UpdateStatistics();
    }

    private void UpdateStatistics()
    {
        FPN_Missing.Count = _materials.Count(x => x.State == ProjectMaterialState.Missing);
        FPN_Replaced.Count = _materials.Count(x => x.State == ProjectMaterialState.Replaced);
        FPN_Collected.Count = _materials.Count(x => x.State == ProjectMaterialState.Collected);
        FPN_Ignored.Count = _materials.Count(x => x.State == ProjectMaterialState.Ignored);

        Dictionary<string, (long RequiredAmount, long CollectedAmount)> groupedMaterials = new(StringComparer.Ordinal);

        foreach(ProjectMaterial material in _materials)
        {
            if(material.State == ProjectMaterialState.Ignored)
                continue;

            var itemId = ItemIdService.Normalize(material.DisplayItemId);
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

    private void LoadMaterialList()
    {
        _materials = _projectMaterialService.GetByProjectId(_project.Id);

        Log.Debug("{LogContext} Project materials loaded. ProjectId={ProjectId}, Count={Count}", LogContext, _project.Id, _materials.Count);
    }

    private void LoadFilterSettings()
    {
        ProjectFilterSettings settings = _projectFilterSettingsService.GetByProjectId(_project.Id);

        _loadingFilterSettings = true;

        try
        {
            FPN_Missing.Checked = settings.ShowMissing;
            FPN_Replaced.Checked = settings.ShowReplaced;
            FPN_Collected.Checked = settings.ShowCollected;
            FPN_Ignored.Checked = settings.ShowIgnored;
        }
        finally
        {
            _loadingFilterSettings = false;
        }
    }

    private void SaveFilterSettings()
    {
        ProjectFilterSettings settings = new()
        {
            ShowMissing = FPN_Missing.Checked,
            ShowReplaced = FPN_Replaced.Checked,
            ShowCollected = FPN_Collected.Checked,
            ShowIgnored = FPN_Ignored.Checked
        };

        _projectFilterSettingsService.Save(_project.Id, settings);
    }

    private void ApplyMaterialView(bool resetScrollPosition)
    {
        CancelPendingScrollUpdate();

        _viewMaterials = _materials
            .Where(ShouldShowMaterial)
            .OrderBy(x => GetStateSortOrder(x.State))
            .ThenByDescending(x => x.RequiredAmount)
            .ToList();

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
            ProjectMaterialState.Missing => FPN_Missing.Checked,
            ProjectMaterialState.Replaced => FPN_Replaced.Checked,
            ProjectMaterialState.Collected => FPN_Collected.Checked,
            ProjectMaterialState.Ignored => FPN_Ignored.Checked,
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
        var contentHeight = PNL_MaterialList.ContentHeight;
        var viewportHeight = Math.Max(1, PNL_MaterialList.ViewportHeight);

        VSB_Main.Minimum = 0;
        VSB_Main.Maximum = Math.Max(0, contentHeight - 1);
        VSB_Main.LargeChange = viewportHeight;
        VSB_Main.SmallChange = PNL_MaterialList.ScrollStep;

        var maximumValue = PNL_MaterialList.MaximumScrollOffset;
        var scrollValue = Math.Clamp(PNL_MaterialList.ScrollOffset, 0, maximumValue);

        if(VSB_Main.Value != scrollValue)
            VSB_Main.Value = scrollValue;

        VSB_Main.Visible = PNL_MaterialList.Visible && maximumValue > 0;
    }

    private void QueueScrollUpdate(int scrollOffset)
    {
        var targetOffset = Math.Clamp(scrollOffset, VSB_Main.Minimum, PNL_MaterialList.MaximumScrollOffset);

        _lastScrollInputTick = Environment.TickCount64;
        PNL_MaterialList.SetToolTipsEnabled(false);

        if(!TMR_ScrollUpdate.Enabled)
            TMR_ScrollUpdate.Start();

        if(_pendingScrollOffset == targetOffset)
            return;

        if(_pendingScrollOffset is null && targetOffset == PNL_MaterialList.ScrollOffset)
            return;

        _pendingScrollOffset = targetOffset;

        if(VSB_Main.Value != targetOffset)
            VSB_Main.Value = targetOffset;
    }

    private void CancelPendingScrollUpdate()
    {
        TMR_ScrollUpdate.Stop();
        _pendingScrollOffset = null;
        PNL_MaterialList.SetToolTipsEnabled(true);
    }

    private void TMR_ScrollUpdate_Tick(object? sender, EventArgs e)
    {
        if(_pendingScrollOffset is int scrollOffset)
        {
            _pendingScrollOffset = null;
            PNL_MaterialList.SetScrollOffset(scrollOffset);
        }

        if(Environment.TickCount64 - _lastScrollInputTick >= 250)
        {
            TMR_ScrollUpdate.Stop();
            PNL_MaterialList.SetToolTipsEnabled(true);
        }
    }

    private void VSB_Main_Scroll(object? sender, ScrollEventArgs e)
    {
        QueueScrollUpdate(e.NewValue);
    }

    private void PNL_MaterialList_MouseWheelScrollRequested(int pixelDelta)
    {
        var currentOffset = _pendingScrollOffset ?? VSB_Main.Value;
        var newOffset = Math.Clamp(currentOffset + pixelDelta, VSB_Main.Minimum, PNL_MaterialList.MaximumScrollOffset);

        QueueScrollUpdate(newOffset);
    }

    private void PNL_MaterialList_ScrollMetricsChanged(object? sender, EventArgs e)
    {
        CancelPendingScrollUpdate();
        UpdateScrollBar();
    }

    private void PNL_MaterialList_ScrollPositionChanged(object? sender, EventArgs e)
    {
        var scrollValue = Math.Clamp(PNL_MaterialList.ScrollOffset, 0, PNL_MaterialList.MaximumScrollOffset);

        if(VSB_Main.Value != scrollValue)
            VSB_Main.Value = scrollValue;
    }

    private void FilterPanel_CheckedChanged(object? sender, EventArgs e)
    {
        if(_loadingFilterSettings)
            return;

        if(sender is FilterPanel filterPanel)
            Log.Debug("{LogContext} Material filter changed. Filter={Filter}, Checked={Checked}", LogContext, filterPanel.FilterStyle, filterPanel.Checked);

        try
        {
            SaveFilterSettings();
        }
        catch(Exception exception)
        {
            Log.Error(exception, "{LogContext} Failed to save project filter settings. ProjectId={ProjectId}", LogContext, _project.Id);

            _ = MessageBox.Show($"The filter settings could not be saved.\n\n{exception.Message}", Constants.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        ApplyMaterialView(resetScrollPosition: true);
    }

    private void IBN_About_Click(object sender, EventArgs e)
    {
        Log.Debug("{LogContext} About dialog requested.", LogContext);

        using FRM_About aboutForm = new();

        _ = aboutForm.ShowDialog(this);
    }

    private void FRM_MaterialList_ResizeBegin(object sender, EventArgs e)
    {
        CancelPendingScrollUpdate();
        PNL_MaterialList.SuspendVirtualLayout();
    }

    private void FRM_MaterialList_ResizeEnd(object sender, EventArgs e)
    {
        CancelPendingScrollUpdate();
        PNL_MaterialList.ResumeVirtualLayout();
        UpdateScrollBar();
    }

    private void BTN_AddMaterial_Click(object? sender, EventArgs e)
    {
        Log.Debug("{LogContext} Add material dialog requested. ProjectId={ProjectId}", LogContext, _project.Id);

        using FRM_AddMaterial addMaterialForm = new(_itemDataProvider, _itemIconService);

        if(addMaterialForm.ShowDialog(this) != DialogResult.OK || string.IsNullOrWhiteSpace(addMaterialForm.SelectedItemId))
        {
            Log.Debug("{LogContext} Add material dialog cancelled. ProjectId={ProjectId}, ItemId={ItemId}", LogContext, _project.Id, addMaterialForm.SelectedItemId);
            return;
        }

        try
        {
            _projectMaterialService.Add(_project.Id, addMaterialForm.SelectedItemId, addMaterialForm.RequiredAmount);

            LoadMaterialList();
            ApplyMaterialView(resetScrollPosition: false);
            UpdateStatistics();

            Log.Information("{LogContext} Material added manually. ProjectId={ProjectId}, ItemId={ItemId}, RequiredAmount={RequiredAmount}", LogContext, _project.Id, addMaterialForm.SelectedItemId, addMaterialForm.RequiredAmount);
        }
        catch(Exception exception)
        {
            Log.Error(exception, "{LogContext} Failed to add material manually. ProjectId={ProjectId}, ItemId={ItemId}", LogContext, _project.Id, addMaterialForm.SelectedItemId);

            _ = MessageBox.Show($"The material could not be added.\n\n{exception.Message}", Constants.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}