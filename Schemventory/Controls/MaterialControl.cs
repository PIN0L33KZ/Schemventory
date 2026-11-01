using Schemventory.App;
using Schemventory.Forms;
using Schemventory.Services;
using Serilog;

namespace Schemventory.Controls;

public partial class MaterialControl : UserControl
{
    private const string LogContext = "(MaterialControl)";

    private readonly ItemIconService _itemIconService;
    private readonly ItemDataProvider _itemDataProvider;
    private readonly ProjectMaterialService _projectMaterialService;
    private readonly System.Windows.Forms.Timer _hoverTimer;

    private bool _isHovered;
    private bool _hasBeenBound;
    private int _bindingVersion;
    private CancellationTokenSource? _bindingCancellationTokenSource;

    public event EventHandler? StateChanged;
    public event Action<ProjectMaterial>? RemoveRequested;

    public ProjectMaterial Material => BoundMaterial ?? throw new InvalidOperationException("No material is currently bound to this control.");

    public ProjectMaterial? BoundMaterial { get; private set; }

    public MaterialControl(ItemIconService itemIconService, ItemDataProvider itemDataProvider, ProjectMaterialService projectMaterialService)
    {
        InitializeComponent();

        _itemIconService = itemIconService;
        _itemDataProvider = itemDataProvider;
        _projectMaterialService = projectMaterialService;

        _hoverTimer = new System.Windows.Forms.Timer
        {
            Interval = 50
        };

        _hoverTimer.Tick += HoverTimer_Tick;
        Disposed += MaterialControl_Disposed;

        RegisterEvents(this);

        PBX_StateIcon.Parent = PBX_MaterialIcon;
        PBX_StateIcon.Location = new Point(PBX_MaterialIcon.ClientSize.Width - PBX_StateIcon.Width, PBX_MaterialIcon.ClientSize.Height - PBX_StateIcon.Height);

        PBX_StateIcon.BringToFront();
    }

    public MaterialControl(ProjectMaterial material, ItemIconService itemIconService, ItemDataProvider itemDataProvider, ProjectMaterialService projectMaterialService) : this(itemIconService, itemDataProvider, projectMaterialService)
    {
        BoundMaterial = material;
    }

    public async Task SetMaterialAsync(ProjectMaterial material)
    {
        ArgumentNullException.ThrowIfNull(material);

        CancelCurrentBinding();

        _bindingCancellationTokenSource = new CancellationTokenSource();

        CancellationToken cancellationToken = _bindingCancellationTokenSource.Token;
        var bindingVersion = ++_bindingVersion;

        BoundMaterial = material;
        _hasBeenBound = true;

        EndHover();
        FillMaterialData();

        PBX_MaterialIcon.Image?.Dispose();
        PBX_MaterialIcon.Image = null;

        try
        {
            await Task.WhenAll(
                LoadItemDataAsync(material, bindingVersion, cancellationToken),
                LoadMaterialIconAsync(material, bindingVersion, cancellationToken));
        }
        catch(OperationCanceledException) when(cancellationToken.IsCancellationRequested)
        {
        }
    }

    public void ClearMaterial()
    {
        CancelCurrentBinding();

        _bindingVersion++;
        _hasBeenBound = false;
        BoundMaterial = null;

        EndHover();

        LBL_MaterialName.Text = string.Empty;
        LBL_MaxStackSize.Text = string.Empty;
        LBL_BlocksNeeded.Text = string.Empty;
        LBL_TotalFormatted.Text = string.Empty;

        PBX_StateIcon.Image = null;
        PBX_StateIcon.Hide();

        PBX_MaterialIcon.Image?.Dispose();
        PBX_MaterialIcon.Image = null;
    }

    private void HoverTimer_Tick(object? sender, EventArgs e)
    {
        if(RectangleToScreen(ClientRectangle).Contains(Cursor.Position))
            return;

        EndHover();
    }

    private async void MaterialControl_Load(object sender, EventArgs e)
    {
        ProjectMaterial? material = BoundMaterial;

        if(material is null || _hasBeenBound)
            return;

        await SetMaterialAsync(material);
    }

    private void MaterialControl_DoubleClick(object? sender, EventArgs e)
    {
        ProjectMaterial? material = BoundMaterial;

        if(material is null)
            return;

        try
        {
            ProjectMaterialState previousState = material.State;

            switch(material.State)
            {
                case ProjectMaterialState.Missing:
                case ProjectMaterialState.Replaced:
                    MarkAsCollected(material);
                    break;

                case ProjectMaterialState.Collected:
                    RestorePreviousState(material);
                    break;

                case ProjectMaterialState.Ignored:
                default:
                    return;
            }

            if(ReferenceEquals(BoundMaterial, material))
                ApplyMaterialState();

            StateChanged?.Invoke(this, EventArgs.Empty);

            Log.Information(
                "{LogContext} Material state changed by double click. ProjectId={ProjectId}, ItemId={ItemId}, PreviousState={PreviousState}, NewState={NewState}",
                LogContext,
                material.ProjectId,
                material.ItemId,
                previousState,
                material.State);
        }
        catch(Exception exception)
        {
            Log.Error(
                exception,
                "{LogContext} Failed to change material state by double click. ProjectId={ProjectId}, ItemId={ItemId}",
                LogContext,
                material.ProjectId,
                material.ItemId);

            _ = MessageBox.Show(
                $"The material state could not be changed.\n\n{exception.Message}",
                Constants.AppName,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void MarkAsCollected(ProjectMaterial material)
    {
        _projectMaterialService.MarkCompleted(material.ProjectId, material.ItemId);

        material.State = ProjectMaterialState.Collected;
        material.CollectedAmount = material.RequiredAmount;
    }

    private void RestorePreviousState(ProjectMaterial material)
    {
        if(!string.IsNullOrWhiteSpace(material.ReplacementItemId))
        {
            _projectMaterialService.MarkReplaced(material.ProjectId, material.ItemId, material.ReplacementItemId);

            material.State = ProjectMaterialState.Replaced;
            material.CollectedAmount = 0;
            return;
        }

        _projectMaterialService.MarkMissing(material.ProjectId, material.ItemId);

        material.State = ProjectMaterialState.Missing;
        material.CollectedAmount = 0;
    }

    private void HoverStart(object? sender, EventArgs e)
    {
        if(BoundMaterial is null || _isHovered)
            return;

        _isHovered = true;

        IBN_MoreOptions.Show();

        PNL_Background.BorderColor = GetStateColor();
        PNL_Background.FillColor3 = GetStateColor();

        _hoverTimer.Start();
    }

    private void MouseRightClick(object? sender, MouseEventArgs e)
    {
        if(BoundMaterial is null || e.Button != MouseButtons.Right)
            return;

        UpdateContextMenuState();

        CMS_MaterialOptions.Show(Cursor.Position);
    }

    private void EndHover()
    {
        _hoverTimer.Stop();
        _isHovered = false;

        IBN_MoreOptions.Hide();

        ApplyDefaultColors();
    }

    private async Task LoadItemDataAsync(ProjectMaterial material, int bindingVersion, CancellationToken cancellationToken)
    {
        var maxStackSize = await _itemDataProvider.GetMaxStackSizeAsync(material.DisplayItemId, cancellationToken);

        if(IsDisposed || cancellationToken.IsCancellationRequested || bindingVersion != _bindingVersion || !ReferenceEquals(BoundMaterial, material))
            return;

        material.MaxStackSize = maxStackSize;

        LBL_MaxStackSize.Text = $"Stack size: {material.MaxStackSize}";

        ApplyMaterialState();
    }

    private void FillMaterialData()
    {
        ProjectMaterial? material = BoundMaterial;

        if(material is null)
            return;

        LBL_MaxStackSize.Text = $"Stack size: {material.MaxStackSize}";
        LBL_BlocksNeeded.Text = $"Total: {material.RequiredAmount}";

        ApplyMaterialState();
    }

    private void ApplyMaterialState()
    {
        ProjectMaterial? material = BoundMaterial;

        if(material is null)
            return;

        ApplyDefaultColors();

        LBL_MaterialName.Text = GetMaterialDisplayName(material);

        switch(material.State)
        {
            case ProjectMaterialState.Missing:
                LBL_TotalFormatted.ForeColor = Constants.MissingColor;
                LBL_TotalFormatted.Text = GetMissingText(material);
                PBX_StateIcon.Image = null;
                PBX_StateIcon.Hide();
                RegisterToolTips(this, "Double click to mark this material as collected");
                break;

            case ProjectMaterialState.Replaced:
                LBL_TotalFormatted.ForeColor = Constants.ReplacedColor;
                LBL_TotalFormatted.Text = GetMissingText(material);
                PBX_StateIcon.Image = Properties.Resources.Switched;
                PBX_StateIcon.Show();
                RegisterToolTips(this, "This material has been replaced");
                break;

            case ProjectMaterialState.Collected:
                LBL_TotalFormatted.ForeColor = Constants.CollectedColor;
                LBL_TotalFormatted.Text = "All collected!";
                PBX_StateIcon.Image = Properties.Resources.Checked;
                PBX_StateIcon.Show();
                RegisterToolTips(this, "Double click to mark this material as missing");
                break;

            case ProjectMaterialState.Ignored:
                LBL_TotalFormatted.ForeColor = Constants.IgnoredColor;
                LBL_TotalFormatted.Text = "Material Ignored";
                PBX_StateIcon.Image = Properties.Resources.Ignored;
                PBX_StateIcon.Show();
                RegisterToolTips(this, "This material is ignored");
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void ApplyDefaultColors()
    {
        PNL_Background.FillColor3 = Constants.DefaultColor;
        PNL_Background.BorderColor = Constants.DefaultBorderColor;
    }

    private Color GetStateColor()
    {
        ProjectMaterial? material = BoundMaterial;

        return material is null
            ? Constants.DefaultColor
            : material.State switch
            {
                ProjectMaterialState.Collected => Constants.CollectedColor,
                ProjectMaterialState.Replaced => Constants.ReplacedColor,
                ProjectMaterialState.Ignored => Constants.IgnoredColor,
                ProjectMaterialState.Missing => Constants.MissingColor,
                _ => Constants.DefaultColor
            };
    }

    private static string GetMaterialDisplayName(ProjectMaterial material)
    {
        var displayName = ItemIdService.GetDisplayName(material.DisplayItemId);

        return !string.IsNullOrWhiteSpace(material.ReplacementItemId)
            ? $"{displayName} (replaced)"
            : displayName;
    }

    private static string GetMissingText(ProjectMaterial material)
    {
        var missingAmount = material.RemainingAmount;
        var stacks = missingAmount / material.MaxStackSize;
        var blocks = missingAmount % material.MaxStackSize;

        var stackText = stacks == 1 ? "Stack" : "Stacks";
        var blockText = blocks == 1 ? "Block" : "Blocks";

        return stacks > 0 && blocks > 0
            ? $"Missing: {stacks} {stackText} + {blocks} {blockText}"
            : stacks > 0
                ? $"Missing: {stacks} {stackText}"
                : $"Missing: {blocks} {blockText}";
    }

    private async Task LoadMaterialIconAsync(ProjectMaterial material, int bindingVersion, CancellationToken cancellationToken)
    {
        try
        {
            Image image = await _itemIconService.GetItemIconAsync(material.DisplayItemId, 64, cancellationToken);

            if(IsDisposed || cancellationToken.IsCancellationRequested || bindingVersion != _bindingVersion || !ReferenceEquals(BoundMaterial, material))
            {
                image.Dispose();
                return;
            }

            PBX_MaterialIcon.Image?.Dispose();
            PBX_MaterialIcon.Image = image;
            PBX_MaterialIcon.SizeMode = PictureBoxSizeMode.Zoom;
        }
        catch(OperationCanceledException)
        {
            throw;
        }
        catch(Exception exception)
        {
            Log.Debug(
                exception,
                "{LogContext} Failed to load material icon. ProjectId={ProjectId}, ItemId={ItemId}",
                LogContext,
                material.ProjectId,
                material.DisplayItemId);

            if(IsDisposed || cancellationToken.IsCancellationRequested || bindingVersion != _bindingVersion || !ReferenceEquals(BoundMaterial, material))
                return;

            PBX_MaterialIcon.Image?.Dispose();
            PBX_MaterialIcon.Image = null;
        }
    }

    private void RegisterEvents(Control control)
    {
        control.MouseEnter += HoverStart;
        control.MouseDown += MouseRightClick;

        if(control != this && control is not Guna.UI2.WinForms.Guna2ImageButton)
            control.DoubleClick += MaterialControl_DoubleClick;

        foreach(Control child in control.Controls)
            RegisterEvents(child);
    }

    private void RegisterToolTips(Control control, string toolTip)
    {
        if(control != this && control is not Guna.UI2.WinForms.Guna2ImageButton)
            TTP_Main.SetToolTip(control, toolTip);

        foreach(Control child in control.Controls)
            RegisterToolTips(child, toolTip);
    }

    private void IBN_MoreOptions_Click(object sender, EventArgs e)
    {
        if(BoundMaterial is null)
            return;

        UpdateContextMenuState();

        CMS_MaterialOptions.Show(IBN_MoreOptions, new Point(0, IBN_MoreOptions.Height));
    }

    private void UpdateContextMenuState()
    {
        ProjectMaterial? material = BoundMaterial;

        if(material is null)
            return;

        TMI_SetStateMissing.Visible = material.State != ProjectMaterialState.Missing;
        TMI_SetStateCollected.Visible = material.State != ProjectMaterialState.Collected;
        TMI_SetStateIgnore.Visible = material.State != ProjectMaterialState.Ignored;
        TMI_SetCollectedAmount.Visible = material.State is not ProjectMaterialState.Collected and not ProjectMaterialState.Ignored;
    }

    private void TMI_SetStateCollected_Click(object sender, EventArgs e)
    {
        SetMaterialState(ProjectMaterialState.Collected);
    }

    private void TMI_SetStateMissing_Click(object sender, EventArgs e)
    {
        SetMaterialState(ProjectMaterialState.Missing);
    }

    private void TMI_SetStateIgnored_Click(object sender, EventArgs e)
    {
        SetMaterialState(ProjectMaterialState.Ignored);
    }

    private void SetMaterialState(ProjectMaterialState state)
    {
        ProjectMaterial? material = BoundMaterial;

        if(material is null)
            return;

        ProjectMaterialState previousState = material.State;

        try
        {
            switch(state)
            {
                case ProjectMaterialState.Missing:
                    _projectMaterialService.MarkMissing(material.ProjectId, material.ItemId);

                    material.CollectedAmount = 0;
                    material.ReplacementItemId = null;
                    break;

                case ProjectMaterialState.Collected:
                    _projectMaterialService.MarkCompleted(material.ProjectId, material.ItemId);

                    material.CollectedAmount = material.RequiredAmount;
                    break;

                case ProjectMaterialState.Ignored:
                    _projectMaterialService.MarkIgnored(material.ProjectId, material.ItemId);

                    material.CollectedAmount = 0;
                    material.ReplacementItemId = null;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }

            material.State = state;

            if(ReferenceEquals(BoundMaterial, material))
                ApplyMaterialState();

            StateChanged?.Invoke(this, EventArgs.Empty);

            Log.Information(
                "{LogContext} Material state changed. ProjectId={ProjectId}, ItemId={ItemId}, PreviousState={PreviousState}, NewState={NewState}",
                LogContext,
                material.ProjectId,
                material.ItemId,
                previousState,
                material.State);
        }
        catch(Exception exception)
        {
            Log.Error(
                exception,
                "{LogContext} Failed to change material state. ProjectId={ProjectId}, ItemId={ItemId}, RequestedState={RequestedState}",
                LogContext,
                material.ProjectId,
                material.ItemId,
                state);

            _ = MessageBox.Show(
                $"The material state could not be changed.\n\n{exception.Message}",
                Constants.AppName,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void TMI_SetCollectedAmount_Click(object sender, EventArgs e)
    {
        ProjectMaterial? material = BoundMaterial;

        if(material is null || material.State == ProjectMaterialState.Ignored)
            return;

        using FRM_AdjustCollectedAmount form = new(material);

        if(form.ShowDialog(this) != DialogResult.OK)
        {
            Log.Debug(
                "{LogContext} Collected amount adjustment cancelled. ProjectId={ProjectId}, ItemId={ItemId}",
                LogContext,
                material.ProjectId,
                material.ItemId);

            return;
        }

        if(form.AmountChange == 0)
            return;

        var newCollectedAmount = Math.Clamp(
            material.CollectedAmount + form.AmountChange,
            0,
            material.RequiredAmount);

        SetCollectedAmount(material, newCollectedAmount);
    }

    private void SetCollectedAmount(ProjectMaterial material, long collectedAmount)
    {
        try
        {
            var previousAmount = material.CollectedAmount;

            _projectMaterialService.UpdateCollectedAmount(
                material.ProjectId,
                material.ItemId,
                collectedAmount);

            material.CollectedAmount = collectedAmount;
            material.State = material.CollectedAmount >= material.RequiredAmount
                ? ProjectMaterialState.Collected
                : !string.IsNullOrWhiteSpace(material.ReplacementItemId)
                    ? ProjectMaterialState.Replaced
                    : ProjectMaterialState.Missing;

            if(ReferenceEquals(BoundMaterial, material))
                ApplyMaterialState();

            StateChanged?.Invoke(this, EventArgs.Empty);

            Log.Information(
                "{LogContext} Collected amount changed. ProjectId={ProjectId}, ItemId={ItemId}, PreviousAmount={PreviousAmount}, NewAmount={NewAmount}",
                LogContext,
                material.ProjectId,
                material.ItemId,
                previousAmount,
                collectedAmount);
        }
        catch(Exception exception)
        {
            Log.Error(
                exception,
                "{LogContext} Failed to change collected amount. ProjectId={ProjectId}, ItemId={ItemId}, RequestedAmount={RequestedAmount}",
                LogContext,
                material.ProjectId,
                material.ItemId,
                collectedAmount);

            _ = MessageBox.Show(
                $"The collected amount could not be changed.\n\n{exception.Message}",
                Constants.AppName,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private async void TMI_ReplaceMaterial_Click(object sender, EventArgs e)
    {
        ProjectMaterial? material = BoundMaterial;

        if(material is null)
            return;

        try
        {
            using FRM_ReplaceMaterial form = new(material, _itemDataProvider, _itemIconService);

            if(form.ShowDialog(this) != DialogResult.OK)
            {
                Log.Debug(
                    "{LogContext} Material replacement cancelled. ProjectId={ProjectId}, ItemId={ItemId}",
                    LogContext,
                    material.ProjectId,
                    material.ItemId);

                return;
            }

            if(form.ResetRequested)
            {
                var previousReplacementItemId = material.ReplacementItemId;

                ResetReplacement(material);

                if(ReferenceEquals(BoundMaterial, material))
                    await SetMaterialAsync(material);

                StateChanged?.Invoke(this, EventArgs.Empty);

                Log.Information(
                    "{LogContext} Material replacement reset. ProjectId={ProjectId}, ItemId={ItemId}, PreviousReplacementItemId={PreviousReplacementItemId}",
                    LogContext,
                    material.ProjectId,
                    material.ItemId,
                    previousReplacementItemId);

                return;
            }

            if(string.IsNullOrWhiteSpace(form.ReplacementItemId))
                return;

            var replacementItemId = form.ReplacementItemId;

            _projectMaterialService.MarkReplaced(
                material.ProjectId,
                material.ItemId,
                replacementItemId);

            material.State = ProjectMaterialState.Replaced;
            material.ReplacementItemId = replacementItemId;
            material.CollectedAmount = 0;

            if(ReferenceEquals(BoundMaterial, material))
                await SetMaterialAsync(material);

            StateChanged?.Invoke(this, EventArgs.Empty);

            Log.Information(
                "{LogContext} Material replaced. ProjectId={ProjectId}, ItemId={ItemId}, ReplacementItemId={ReplacementItemId}",
                LogContext,
                material.ProjectId,
                material.ItemId,
                replacementItemId);
        }
        catch(Exception exception)
        {
            Log.Error(
                exception,
                "{LogContext} Failed to replace material. ProjectId={ProjectId}, ItemId={ItemId}",
                LogContext,
                material.ProjectId,
                material.ItemId);

            _ = MessageBox.Show(
                $"The material could not be replaced.\n\n{exception.Message}",
                Constants.AppName,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void CancelCurrentBinding()
    {
        if(_bindingCancellationTokenSource is null)
            return;

        _bindingCancellationTokenSource.Cancel();
        _bindingCancellationTokenSource.Dispose();
        _bindingCancellationTokenSource = null;
    }

    private void ResetReplacement(ProjectMaterial material)
    {
        _projectMaterialService.MarkMissing(material.ProjectId, material.ItemId);

        material.State = ProjectMaterialState.Missing;
        material.ReplacementItemId = null;
        material.CollectedAmount = 0;
    }

    private void MaterialControl_Disposed(object? sender, EventArgs e)
    {
        CancelCurrentBinding();

        _bindingVersion++;

        _hoverTimer.Stop();
        _hoverTimer.Dispose();

        PBX_MaterialIcon.Image?.Dispose();
        PBX_MaterialIcon.Image = null;
    }

    private void TMI_RemoveMaterial_Click(object sender, EventArgs e)
    {
        ProjectMaterial? material = BoundMaterial;

        if(material is null)
            return;

        RemoveRequested?.Invoke(material);
    }
}