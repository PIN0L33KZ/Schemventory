using Schemventory.Services;

namespace Schemventory.Controls;

public sealed class VirtualMaterialPanel : Panel
{
    private const int MaterialControlWidth = 312;
    private const int MaterialControlHeight = 97;
    private const int HorizontalGap = 6;
    private const int VerticalGap = 6;
    private const int ContentPadding = 3;

    private readonly List<MaterialControl> _controlPool = [];

    private IReadOnlyList<ProjectMaterial> _materials = Array.Empty<ProjectMaterial>();

    private ItemIconService? _itemIconService;
    private ItemDataProvider? _itemDataProvider;
    private ProjectMaterialService? _projectMaterialService;

    private int _columnCount = 1;
    private int _refreshVersion;
    private bool _resizeSuspended;

    public event EventHandler? MaterialStateChanged;
    public event EventHandler? ScrollMetricsChanged;
    public event EventHandler? ScrollPositionChanged;
    public event Action<int>? MouseWheelScrollRequested;

    public int TotalRows => _materials.Count == 0 ? 0 : (int)Math.Ceiling((double)_materials.Count / _columnCount);
    public int ViewportRows { get; private set; } = 1;
    public int MaximumScrollRow => Math.Max(0, TotalRows - ViewportRows);
    public int ScrollRow { get; private set; }

    public VirtualMaterialPanel()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);

        UpdateStyles();
        TabStop = true;
    }

    public void Initialize(ItemIconService itemIconService, ItemDataProvider itemDataProvider, ProjectMaterialService projectMaterialService)
    {
        _itemIconService = itemIconService;
        _itemDataProvider = itemDataProvider;
        _projectMaterialService = projectMaterialService;

        RecalculateLayout();
    }

    public void SetMaterials(IReadOnlyList<ProjectMaterial> materials, bool resetScrollPosition)
    {
        ArgumentNullException.ThrowIfNull(materials);

        _materials = materials;
        ScrollRow = resetScrollPosition ? 0 : Math.Min(ScrollRow, MaximumScrollRow);

        RecalculateLayout();
        RefreshVisibleControls();

        ScrollMetricsChanged?.Invoke(this, EventArgs.Empty);
        ScrollPositionChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetScrollRow(int scrollRow)
    {
        var newScrollRow = Math.Clamp(scrollRow, 0, MaximumScrollRow);

        if(newScrollRow == ScrollRow)
            return;

        ScrollRow = newScrollRow;

        RefreshVisibleControls();
        ScrollPositionChanged?.Invoke(this, EventArgs.Empty);
    }

    protected override void OnResize(EventArgs eventArgs)
    {
        base.OnResize(eventArgs);

        if(_itemIconService is null || _resizeSuspended)
            return;

        var oldColumnCount = _columnCount;
        var oldViewportRowCount = ViewportRows;

        RecalculateLayout();

        ScrollRow = Math.Min(ScrollRow, MaximumScrollRow);

        if(oldColumnCount != _columnCount || oldViewportRowCount != ViewportRows)
        {
            RefreshVisibleControls();
            ScrollMetricsChanged?.Invoke(this, EventArgs.Empty);
            ScrollPositionChanged?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            PositionControlPool();
        }
    }

    protected override void OnMouseWheel(MouseEventArgs eventArgs)
    {
        base.OnMouseWheel(eventArgs);
        ScrollByMouseWheel(eventArgs.Delta);
    }

    private void RecalculateLayout()
    {
        if(_itemIconService is null || _itemDataProvider is null || _projectMaterialService is null)
            return;

        var availableWidth = Math.Max(MaterialControlWidth, ClientSize.Width);

        _columnCount = Math.Max(1, (availableWidth - (ContentPadding * 2) + HorizontalGap) / (MaterialControlWidth + HorizontalGap));

        var availableHeight = Math.Max(MaterialControlHeight, ClientSize.Height - (ContentPadding * 2));

        ViewportRows = Math.Max(1, (availableHeight + VerticalGap) / (MaterialControlHeight + VerticalGap));

        var requiredPoolSize = _columnCount * ViewportRows;

        EnsureControlPoolSize(requiredPoolSize);
        PositionControlPool();
    }

    private void EnsureControlPoolSize(int requiredPoolSize)
    {
        if(_itemIconService is null || _itemDataProvider is null || _projectMaterialService is null)
            return;

        while(_controlPool.Count < requiredPoolSize)
        {
            MaterialControl materialControl = new(_itemIconService, _itemDataProvider, _projectMaterialService)
            {
                Visible = false
            };

            materialControl.StateChanged += MaterialControl_StateChanged;
            RegisterMouseWheel(materialControl);

            _controlPool.Add(materialControl);
            Controls.Add(materialControl);
        }

        for(var i = requiredPoolSize; i < _controlPool.Count; i++)
        {
            _controlPool[i].ClearMaterial();
            _controlPool[i].Visible = false;
        }
    }

    private void PositionControlPool()
    {
        if(_controlPool.Count == 0)
            return;

        var contentWidth = (_columnCount * MaterialControlWidth) + ((_columnCount - 1) * HorizontalGap);
        var startX = Math.Max(ContentPadding, (ClientSize.Width - contentWidth) / 2);

        SuspendLayout();

        try
        {
            for(var index = 0; index < _controlPool.Count; index++)
            {
                var row = index / _columnCount;
                var column = index % _columnCount;

                _controlPool[index].Location = new Point(startX + (column * (MaterialControlWidth + HorizontalGap)), ContentPadding + (row * (MaterialControlHeight + VerticalGap)));
                _controlPool[index].Size = new Size(MaterialControlWidth, MaterialControlHeight);
            }
        }
        finally
        {
            ResumeLayout(false);
        }
    }

    private void RefreshVisibleControls()
    {
        if(_itemIconService is null)
            return;

        var refreshVersion = ++_refreshVersion;
        var firstMaterialIndex = ScrollRow * _columnCount;

        List<Task> bindingTasks = [];

        SuspendLayout();

        try
        {
            for(var slotIndex = 0; slotIndex < _controlPool.Count; slotIndex++)
            {
                MaterialControl materialControl = _controlPool[slotIndex];
                var materialIndex = firstMaterialIndex + slotIndex;

                if(materialIndex >= _materials.Count)
                {
                    materialControl.ClearMaterial();
                    materialControl.Visible = false;
                    continue;
                }

                ProjectMaterial material = _materials[materialIndex];

                materialControl.Visible = true;

                if(ReferenceEquals(materialControl.BoundMaterial, material))
                    continue;

                bindingTasks.Add(materialControl.SetMaterialAsync(material));
            }
        }
        finally
        {
            ResumeLayout(false);
        }

        if(bindingTasks.Count > 0)
            _ = CompleteRefreshAsync(bindingTasks, refreshVersion);
    }

    private async Task CompleteRefreshAsync(IReadOnlyCollection<Task> bindingTasks, int refreshVersion)
    {
        try
        {
            await Task.WhenAll(bindingTasks);

            if(IsDisposed || refreshVersion != _refreshVersion)
                return;

            Invalidate();
        }
        catch(OperationCanceledException)
        {
        }
    }

    private void MaterialControl_StateChanged(object? sender, EventArgs e)
    {
        MaterialStateChanged?.Invoke(sender, e);
    }

    private void RegisterMouseWheel(Control control)
    {
        control.MouseWheel += ChildControl_MouseWheel;

        foreach(Control child in control.Controls)
            RegisterMouseWheel(child);
    }

    private void ChildControl_MouseWheel(object? sender, MouseEventArgs e)
    {
        ScrollByMouseWheel(e.Delta);
    }

    private void ScrollByMouseWheel(int delta)
    {
        if(delta == 0 || MaximumScrollRow == 0)
            return;

        var wheelSteps = Math.Max(1, Math.Abs(delta) / SystemInformation.MouseWheelScrollDelta);
        var direction = delta > 0 ? -1 : 1;

        MouseWheelScrollRequested?.Invoke(direction * wheelSteps);
    }

    public void SuspendVirtualLayout()
    {
        _resizeSuspended = true;
    }

    public void ResumeVirtualLayout()
    {
        _resizeSuspended = false;

        RecalculateLayout();

        ScrollRow = Math.Min(ScrollRow, MaximumScrollRow);

        RefreshVisibleControls();

        ScrollMetricsChanged?.Invoke(this, EventArgs.Empty);
        ScrollPositionChanged?.Invoke(this, EventArgs.Empty);
    }
}