using Serilog;
using Schemventory.App;
using Schemventory.Forms;
using Schemventory.Services;

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
            await Task.WhenAll(LoadItemDataAsync(material, bindingVersion, cancellationToken), LoadMaterialIconAsync(material, bindingVersion, cancellationToken));
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
        if(BoundMaterial is null || _hasBeenBound)
            return;

        await SetMaterialAsync(BoundMaterial);
    }

    private void MaterialControl_DoubleClick(object? sender, EventArgs e)
    {
        if(BoundMaterial is null)
            return;

        try
        {
            ProjectMaterialState previousState = Material.State;

            switch(Material.State)
            {
                case ProjectMaterialState.Missing:
                case ProjectMaterialState.Replaced:
                    MarkAsCollected();
                    break;

                case ProjectMaterialState.Collected:
                    RestorePreviousState();
                    break;

                case ProjectMaterialState.Ignored:
                default:
                    return;
            }

            ApplyMaterialState();
            StateChanged?.Invoke(this, EventArgs.Empty);

            Log.Information("{LogContext} Material state changed by double click. ProjectId={ProjectId}, ItemId={ItemId}, PreviousState={PreviousState}, NewState={NewState}", LogContext, Material.ProjectId, Material.ItemId, previousState, Material.State);
        }
        catch(Exception exception)
        {
            Log.Error(exception, "{LogContext} Failed to change material state by double click. ProjectId={ProjectId}, ItemId={ItemId}", LogContext, Material.ProjectId, Material.ItemId);
            _ = MessageBox.Show($"The material state could not be changed.\n\n{exception.Message}", Constants.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void MarkAsCollected()
    {
        _projectMaterialService.MarkCompleted(Material.ProjectId, Material.ItemId);

        Material.State = ProjectMaterialState.Collected;
        Material.CollectedAmount = Material.RequiredAmount;
    }

    private void RestorePreviousState()
    {
        if(!string.IsNullOrWhiteSpace(Material.ReplacementItemId))
        {
            _projectMaterialService.MarkReplaced(Material.ProjectId, Material.ItemId, Material.ReplacementItemId);

            Material.State = ProjectMaterialState.Replaced;
            Material.CollectedAmount = 0;
            return;
        }

        _projectMaterialService.MarkMissing(Material.ProjectId, Material.ItemId);

        Material.State = ProjectMaterialState.Missing;
        Material.CollectedAmount = 0;
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
        if(BoundMaterial is null)
            return;

        LBL_MaxStackSize.Text = $"Stack size: {Material.MaxStackSize}";
        LBL_BlocksNeeded.Text = $"Total: {Material.RequiredAmount}";

        ApplyMaterialState();
    }

    private void ApplyMaterialState()
    {
        if(BoundMaterial is null)
            return;

        ApplyDefaultColors();

        LBL_MaterialName.Text = GetMaterialDisplayName();

        switch(Material.State)
        {
            case ProjectMaterialState.Missing:
                LBL_TotalFormatted.ForeColor = Constants.MissingColor;
                LBL_TotalFormatted.Text = GetMissingText();
                PBX_StateIcon.Image = null;
                PBX_StateIcon.Hide();
                RegisterToolTips(this, "Double click to mark this material as collected");
                break;

            case ProjectMaterialState.Replaced:
                LBL_TotalFormatted.ForeColor = Constants.ReplacedColor;
                LBL_TotalFormatted.Text = GetMissingText();
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
        return BoundMaterial is null
            ? Constants.DefaultColor
            : Material.State switch
            {
                ProjectMaterialState.Collected => Constants.CollectedColor,
                ProjectMaterialState.Replaced => Constants.ReplacedColor,
                ProjectMaterialState.Ignored => Constants.IgnoredColor,
                ProjectMaterialState.Missing => Constants.MissingColor,
                _ => Constants.DefaultColor
            };
    }

    private string GetMaterialDisplayName()
    {
        var displayName = GetDisplayName(Material.DisplayItemId);

        return !string.IsNullOrWhiteSpace(Material.ReplacementItemId) ? $"{displayName} (replaced)" : displayName;
    }

    private string GetMissingText()
    {
        var missingAmount = Material.RemainingAmount;
        var stacks = missingAmount / Material.MaxStackSize;
        var blocks = missingAmount % Material.MaxStackSize;

        var stackText = stacks == 1 ? "Stack" : "Stacks";
        var blockText = blocks == 1 ? "Block" : "Blocks";

        return stacks > 0 && blocks > 0 ? $"Missing: {stacks} {stackText} + {blocks} {blockText}" : stacks > 0 ? $"Missing: {stacks} {stackText}" : $"Missing: {blocks} {blockText}";
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
            Log.Debug(exception, "{LogContext} Failed to load material icon. ProjectId={ProjectId}, ItemId={ItemId}", LogContext, material.ProjectId, material.DisplayItemId);

            if(IsDisposed || cancellationToken.IsCancellationRequested || bindingVersion != _bindingVersion || !ReferenceEquals(BoundMaterial, material))
                return;

            PBX_MaterialIcon.Image?.Dispose();
            PBX_MaterialIcon.Image = null;
        }
    }

    private static string GetDisplayName(string itemId)
    {
        var name = itemId.Replace("minecraft:", "", StringComparison.Ordinal);

        name = name.Replace('_', ' ');

        return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name);
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
        TMI_SetStateMissing.Visible = Material.State != ProjectMaterialState.Missing;
        TMI_SetStateCollected.Visible = Material.State != ProjectMaterialState.Collected;
        TMI_SetStateIgnore.Visible = Material.State != ProjectMaterialState.Ignored;
        TMI_SetCollectedAmount.Visible = Material.State is not ProjectMaterialState.Collected and not ProjectMaterialState.Ignored;
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
        if(BoundMaterial is null)
            return;

        ProjectMaterialState previousState = Material.State;

        try
        {
            switch(state)
            {
                case ProjectMaterialState.Missing:
                    _projectMaterialService.MarkMissing(Material.ProjectId, Material.ItemId);

                    Material.CollectedAmount = 0;
                    Material.ReplacementItemId = null;
                    break;

                case ProjectMaterialState.Collected:
                    _projectMaterialService.MarkCompleted(Material.ProjectId, Material.ItemId);

                    Material.CollectedAmount = Material.RequiredAmount;
                    break;

                case ProjectMaterialState.Ignored:
                    _projectMaterialService.MarkIgnored(Material.ProjectId, Material.ItemId);

                    Material.CollectedAmount = 0;
                    Material.ReplacementItemId = null;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }

            Material.State = state;

            ApplyMaterialState();
            StateChanged?.Invoke(this, EventArgs.Empty);

            Log.Information("{LogContext} Material state changed. ProjectId={ProjectId}, ItemId={ItemId}, PreviousState={PreviousState}, NewState={NewState}", LogContext, Material.ProjectId, Material.ItemId, previousState, Material.State);
        }
        catch(Exception exception)
        {
            Log.Error(exception, "{LogContext} Failed to change material state. ProjectId={ProjectId}, ItemId={ItemId}, RequestedState={RequestedState}", LogContext, Material.ProjectId, Material.ItemId, state);
            _ = MessageBox.Show($"The material state could not be changed.\n\n{exception.Message}", Constants.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void TMI_SetCollectedAmount_Click(object sender, EventArgs e)
    {
        if(BoundMaterial is null || Material.State == ProjectMaterialState.Ignored)
            return;

        using FRM_AdjustCollectedAmount form = new(Material);

        if(form.ShowDialog(this) != DialogResult.OK)
        {
            Log.Debug("{LogContext} Collected amount adjustment cancelled. ProjectId={ProjectId}, ItemId={ItemId}", LogContext, Material.ProjectId, Material.ItemId);
            return;
        }

        if(form.AmountChange == 0)
            return;

        var newCollectedAmount = Math.Clamp(Material.CollectedAmount + form.AmountChange, 0, Material.RequiredAmount);

        SetCollectedAmount(newCollectedAmount);
    }

    private void SetCollectedAmount(long collectedAmount)
    {
        try
        {
            var previousAmount = Material.CollectedAmount;

            _projectMaterialService.UpdateCollectedAmount(Material.ProjectId, Material.ItemId, collectedAmount);

            Material.CollectedAmount = collectedAmount;
            Material.State = Material.CollectedAmount >= Material.RequiredAmount ? ProjectMaterialState.Collected : !string.IsNullOrWhiteSpace(Material.ReplacementItemId) ? ProjectMaterialState.Replaced : ProjectMaterialState.Missing;

            ApplyMaterialState();
            StateChanged?.Invoke(this, EventArgs.Empty);

            Log.Information("{LogContext} Collected amount changed. ProjectId={ProjectId}, ItemId={ItemId}, PreviousAmount={PreviousAmount}, NewAmount={NewAmount}", LogContext, Material.ProjectId, Material.ItemId, previousAmount, collectedAmount);
        }
        catch(Exception exception)
        {
            Log.Error(exception, "{LogContext} Failed to change collected amount. ProjectId={ProjectId}, ItemId={ItemId}, RequestedAmount={RequestedAmount}", LogContext, Material.ProjectId, Material.ItemId, collectedAmount);
            _ = MessageBox.Show($"The collected amount could not be changed.\n\n{exception.Message}", Constants.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void TMI_ReplaceMaterial_Click(object sender, EventArgs e)
    {
        if(BoundMaterial is null)
            return;

        try
        {
            using FRM_ReplaceMaterial form = new(Material, _itemDataProvider, _itemIconService);

            if(form.ShowDialog(this) != DialogResult.OK)
            {
                Log.Debug("{LogContext} Material replacement cancelled. ProjectId={ProjectId}, ItemId={ItemId}", LogContext, Material.ProjectId, Material.ItemId);
                return;
            }

            if(form.ResetRequested)
            {
                var previousReplacementItemId = Material.ReplacementItemId;

                ResetReplacement();

                await RefreshMaterialAsync();

                StateChanged?.Invoke(this, EventArgs.Empty);

                Log.Information("{LogContext} Material replacement reset. ProjectId={ProjectId}, ItemId={ItemId}, PreviousReplacementItemId={PreviousReplacementItemId}", LogContext, Material.ProjectId, Material.ItemId, previousReplacementItemId);
                return;
            }

            if(string.IsNullOrWhiteSpace(form.ReplacementItemId))
                return;

            var replacementItemId = form.ReplacementItemId;

            _projectMaterialService.MarkReplaced(Material.ProjectId, Material.ItemId, replacementItemId);

            Material.State = ProjectMaterialState.Replaced;
            Material.ReplacementItemId = replacementItemId;
            Material.CollectedAmount = 0;

            await RefreshMaterialAsync();

            StateChanged?.Invoke(this, EventArgs.Empty);

            Log.Information("{LogContext} Material replaced. ProjectId={ProjectId}, ItemId={ItemId}, ReplacementItemId={ReplacementItemId}", LogContext, Material.ProjectId, Material.ItemId, replacementItemId);
        }
        catch(Exception exception)
        {
            Log.Error(exception, "{LogContext} Failed to replace material. ProjectId={ProjectId}, ItemId={ItemId}", LogContext, Material.ProjectId, Material.ItemId);
            _ = MessageBox.Show($"The material could not be replaced.\n\n{exception.Message}", Constants.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task RefreshMaterialAsync()
    {
        if(BoundMaterial is null)
            return;

        await SetMaterialAsync(BoundMaterial);
    }

    private void CancelCurrentBinding()
    {
        if(_bindingCancellationTokenSource is null)
            return;

        _bindingCancellationTokenSource.Cancel();
        _bindingCancellationTokenSource.Dispose();
        _bindingCancellationTokenSource = null;
    }

    private void ResetReplacement()
    {
        _projectMaterialService.MarkMissing(Material.ProjectId, Material.ItemId);

        Material.State = ProjectMaterialState.Missing;
        Material.ReplacementItemId = null;
        Material.CollectedAmount = 0;
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
}
