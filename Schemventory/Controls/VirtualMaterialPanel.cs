using Schemventory.Services;
using Serilog;

namespace Schemventory.Controls;

public sealed class VirtualMaterialPanel : Panel
{
    private const string LogContext = "(VirtualMaterialPanel)";
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

    private Bitmap? _resizeSnapshot;
    private int _columnCount = 1;
    private int _renderRowCount = 1;
    private int _refreshVersion;
    private bool _resizeSuspended;

    public event EventHandler? MaterialStateChanged;
    public event EventHandler? ScrollMetricsChanged;
    public event EventHandler? ScrollPositionChanged;
    public event Action<int>? MouseWheelScrollRequested;
    public event Action<ProjectMaterial>? MaterialRemoveRequested;

    public int TotalRows => _materials.Count == 0 ? 0 : (int)Math.Ceiling((double)_materials.Count / _columnCount);
    public int ViewportRows { get; private set; } = 1;
    public int MaximumScrollRow => Math.Max(0, TotalRows - ViewportRows);
    public int ScrollRow { get; private set; }

    public VirtualMaterialPanel()
    {
        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.UserPaint,
            true);

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

        if(_resizeSuspended)
        {
            if(resetScrollPosition)
                ScrollRow = 0;

            return;
        }

        RecalculateLayout();

        ScrollRow = resetScrollPosition ? 0 : Math.Min(ScrollRow, MaximumScrollRow);

        RefreshVisibleControls();

        ScrollMetricsChanged?.Invoke(this, EventArgs.Empty);
        ScrollPositionChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetScrollRow(int scrollRow)
    {
        if(_resizeSuspended)
            return;

        var newScrollRow = Math.Clamp(scrollRow, 0, MaximumScrollRow);

        if(newScrollRow == ScrollRow)
            return;

        var oldScrollRow = ScrollRow;
        ScrollRow = newScrollRow;

        if(!TryRecycleRows(oldScrollRow, newScrollRow))
            RefreshVisibleControls();

        ScrollPositionChanged?.Invoke(this, EventArgs.Empty);
    }

    protected override void OnResize(EventArgs eventArgs)
    {
        base.OnResize(eventArgs);

        if(_itemIconService is null)
            return;

        if(_resizeSuspended)
        {
            Invalidate();
            return;
        }

        var oldColumnCount = _columnCount;
        var oldViewportRowCount = ViewportRows;
        var oldRenderRowCount = _renderRowCount;

        RecalculateLayout();

        ScrollRow = Math.Min(ScrollRow, MaximumScrollRow);

        if(oldColumnCount != _columnCount ||
           oldViewportRowCount != ViewportRows ||
           oldRenderRowCount != _renderRowCount)
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

    protected override void OnPaint(PaintEventArgs eventArgs)
    {
        base.OnPaint(eventArgs);

        if(!_resizeSuspended ||
           _resizeSnapshot is null ||
           ClientSize.Width <= 0 ||
           ClientSize.Height <= 0)
        {
            return;
        }

        // Keep the captured material list at its original size during resize.
        // Enlarged areas are painted with the panel background instead of
        // stretching the snapshot and distorting the material controls.
        eventArgs.Graphics.DrawImageUnscaled(
            _resizeSnapshot,
            Point.Empty);
    }

    protected override void OnMouseWheel(MouseEventArgs eventArgs)
    {
        base.OnMouseWheel(eventArgs);

        if(_resizeSuspended)
            return;

        ScrollByMouseWheel(eventArgs.Delta);
    }

    protected override void Dispose(bool disposing)
    {
        if(disposing)
        {
            _resizeSnapshot?.Dispose();
            _resizeSnapshot = null;
        }

        base.Dispose(disposing);
    }

    private void RecalculateLayout()
    {
        if(_itemIconService is null || _itemDataProvider is null || _projectMaterialService is null)
            return;

        var availableWidth = Math.Max(MaterialControlWidth, ClientSize.Width);

        _columnCount = Math.Max(
            1,
            (availableWidth - (ContentPadding * 2) + HorizontalGap) /
            (MaterialControlWidth + HorizontalGap));

        var availableHeight = Math.Max(
            MaterialControlHeight,
            ClientSize.Height - (ContentPadding * 2));

        var rowHeight = MaterialControlHeight + VerticalGap;

        ViewportRows = Math.Max(
            1,
            (availableHeight + VerticalGap) / rowHeight);

        _renderRowCount = Math.Max(
            ViewportRows,
            (int)Math.Ceiling((double)availableHeight / rowHeight));

        EnsureControlPoolSize(_columnCount * _renderRowCount);
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
            materialControl.RemoveRequested += MaterialControl_RemoveRequested;

            RegisterMouseWheel(materialControl);

            _controlPool.Add(materialControl);
            Controls.Add(materialControl);
        }

        while(_controlPool.Count > requiredPoolSize)
        {
            var lastIndex = _controlPool.Count - 1;
            MaterialControl materialControl = _controlPool[lastIndex];

            materialControl.StateChanged -= MaterialControl_StateChanged;
            materialControl.RemoveRequested -= MaterialControl_RemoveRequested;

            Controls.Remove(materialControl);
            _controlPool.RemoveAt(lastIndex);

            materialControl.Dispose();
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

                _controlPool[index].Location = new Point(
                    startX + (column * (MaterialControlWidth + HorizontalGap)),
                    ContentPadding + (row * (MaterialControlHeight + VerticalGap)));

                _controlPool[index].Size = new Size(
                    MaterialControlWidth,
                    MaterialControlHeight);
            }
        }
        finally
        {
            ResumeLayout(false);
        }
    }

    private bool TryRecycleRows(int oldScrollRow, int newScrollRow)
    {
        if(_resizeSuspended)
            return false;

        var rowDelta = newScrollRow - oldScrollRow;
        var rowsToRecycle = Math.Abs(rowDelta);

        if(rowDelta == 0 || rowsToRecycle >= _renderRowCount || _controlPool.Count == 0)
            return false;

        var controlsToRecycle = rowsToRecycle * _columnCount;

        if(controlsToRecycle <= 0 || controlsToRecycle >= _controlPool.Count)
            return false;

        if(rowDelta > 0)
        {
            List<MaterialControl> recycledControls = _controlPool.GetRange(0, controlsToRecycle);

            _controlPool.RemoveRange(0, controlsToRecycle);
            _controlPool.AddRange(recycledControls);
        }
        else
        {
            var startIndex = _controlPool.Count - controlsToRecycle;
            List<MaterialControl> recycledControls = _controlPool.GetRange(startIndex, controlsToRecycle);

            _controlPool.RemoveRange(startIndex, controlsToRecycle);
            _controlPool.InsertRange(0, recycledControls);
        }

        PositionControlPool();

        var refreshVersion = ++_refreshVersion;
        var firstMaterialIndex = ScrollRow * _columnCount;
        var firstSlotToBind = rowDelta > 0 ? _controlPool.Count - controlsToRecycle : 0;
        var lastSlotToBind = rowDelta > 0 ? _controlPool.Count : controlsToRecycle;

        List<Task> bindingTasks = [];

        for(var slotIndex = firstSlotToBind; slotIndex < lastSlotToBind; slotIndex++)
            BindControl(_controlPool[slotIndex], firstMaterialIndex + slotIndex, bindingTasks);

        if(bindingTasks.Count > 0)
            _ = CompleteRefreshAsync(bindingTasks, refreshVersion);

        return true;
    }

    private void RefreshVisibleControls(bool forceRebind = false)
    {
        if(_itemIconService is null || _resizeSuspended)
            return;

        var refreshVersion = ++_refreshVersion;
        var firstMaterialIndex = ScrollRow * _columnCount;

        List<Task> bindingTasks = [];

        SuspendLayout();

        try
        {
            for(var slotIndex = 0; slotIndex < _controlPool.Count; slotIndex++)
            {
                BindControl(
                    _controlPool[slotIndex],
                    firstMaterialIndex + slotIndex,
                    bindingTasks,
                    forceRebind);
            }
        }
        finally
        {
            ResumeLayout(false);
        }

        if(bindingTasks.Count > 0)
            _ = CompleteRefreshAsync(bindingTasks, refreshVersion);
    }

    private void BindControl(MaterialControl materialControl, int materialIndex, List<Task> bindingTasks, bool forceRebind = false)
    {
        if(_resizeSuspended)
            return;

        if(materialIndex >= _materials.Count)
        {
            materialControl.ClearMaterial();
            materialControl.Visible = false;
            return;
        }

        ProjectMaterial material = _materials[materialIndex];

        materialControl.Visible = true;

        if(!forceRebind &&
           ReferenceEquals(materialControl.BoundMaterial, material))
        {
            return;
        }

        bindingTasks.Add(materialControl.SetMaterialAsync(material));
    }

    private async Task CompleteRefreshAsync(IReadOnlyCollection<Task> bindingTasks, int refreshVersion)
    {
        try
        {
            await Task.WhenAll(bindingTasks);

            if(IsDisposed ||
               _resizeSuspended ||
               refreshVersion != _refreshVersion)
            {
                return;
            }

            Invalidate();
        }
        catch(OperationCanceledException)
        {
        }
        catch(Exception exception)
        {
            if(IsDisposed ||
               _resizeSuspended ||
               refreshVersion != _refreshVersion)
            {
                Log.Debug(
                    exception,
                    "{LogContext} Ignoring failed material refresh because it is no longer current. RefreshVersion={RefreshVersion}, CurrentRefreshVersion={CurrentRefreshVersion}, ResizeSuspended={ResizeSuspended}",
                    LogContext,
                    refreshVersion,
                    _refreshVersion,
                    _resizeSuspended);

                return;
            }

            Log.Warning(
                exception,
                "{LogContext} Failed to refresh visible material controls. RefreshVersion={RefreshVersion}",
                LogContext,
                refreshVersion);
        }
    }

    private void MaterialControl_StateChanged(object? sender, EventArgs e)
    {
        MaterialStateChanged?.Invoke(sender, e);
    }

    private void MaterialControl_RemoveRequested(ProjectMaterial material)
    {
        MaterialRemoveRequested?.Invoke(material);
    }

    private void RegisterMouseWheel(Control control)
    {
        control.MouseWheel += ChildControl_MouseWheel;

        foreach(Control child in control.Controls)
            RegisterMouseWheel(child);
    }

    private void ChildControl_MouseWheel(object? sender, MouseEventArgs e)
    {
        if(_resizeSuspended)
            return;

        ScrollByMouseWheel(e.Delta);
    }

    private void ScrollByMouseWheel(int delta)
    {
        if(_resizeSuspended || delta == 0 || MaximumScrollRow == 0)
            return;

        var wheelSteps = Math.Max(1, Math.Abs(delta) / SystemInformation.MouseWheelScrollDelta);
        var direction = delta > 0 ? -1 : 1;

        MouseWheelScrollRequested?.Invoke(direction * wheelSteps);
    }

    private void CreateResizeSnapshot()
    {
        _resizeSnapshot?.Dispose();
        _resizeSnapshot = null;

        if(ClientSize.Width <= 0 || ClientSize.Height <= 0)
            return;

        Bitmap snapshot = new(ClientSize.Width, ClientSize.Height);

        try
        {
            DrawToBitmap(snapshot, new Rectangle(Point.Empty, ClientSize));
            _resizeSnapshot = snapshot;
        }
        catch
        {
            snapshot.Dispose();
            throw;
        }
    }

    public void SuspendVirtualLayout()
    {
        if(_resizeSuspended)
            return;

        CreateResizeSnapshot();

        _resizeSuspended = true;
        _refreshVersion++;

        SuspendLayout();

        try
        {
            foreach(MaterialControl materialControl in _controlPool)
            {
                materialControl.SuspendBinding();
                materialControl.Visible = false;
            }
        }
        finally
        {
            ResumeLayout(false);
        }

        Invalidate();

        Log.Debug(
            "{LogContext} Virtual layout suspended with resize snapshot. ControlCount={ControlCount}, RefreshVersion={RefreshVersion}, SnapshotAvailable={SnapshotAvailable}",
            LogContext,
            _controlPool.Count,
            _refreshVersion,
            _resizeSnapshot is not null);
    }

    public void ResumeVirtualLayout()
    {
        if(!_resizeSuspended)
            return;

        _resizeSuspended = false;

        RecalculateLayout();

        ScrollRow = Math.Min(ScrollRow, MaximumScrollRow);

        RefreshVisibleControls(forceRebind: true);

        _resizeSnapshot?.Dispose();
        _resizeSnapshot = null;

        ScrollMetricsChanged?.Invoke(this, EventArgs.Empty);
        ScrollPositionChanged?.Invoke(this, EventArgs.Empty);

        Invalidate();

        Log.Debug(
            "{LogContext} Virtual layout resumed after snapshot resize. ColumnCount={ColumnCount}, ViewportRows={ViewportRows}, RenderRowCount={RenderRowCount}, ScrollRow={ScrollRow}",
            LogContext,
            _columnCount,
            ViewportRows,
            _renderRowCount,
            ScrollRow);
    }
}