using Schemventory.App;
using Schemventory.Services;
using Serilog;
using System.Drawing.Drawing2D;

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
    private readonly DragPlaceholder _dragPlaceholder = new();
    private readonly System.Windows.Forms.Timer _dragTimer = new() { Interval = Constants.MaterialDragUpdateIntervalMilliseconds };
    private readonly DragCancelFilter _dragCancelFilter;

    private IReadOnlyList<ProjectMaterial> _materials = Array.Empty<ProjectMaterial>();

    private ItemIconService? _itemIconService;
    private ItemDataProvider? _itemDataProvider;
    private ProjectMaterialService? _projectMaterialService;

    private Bitmap? _resizeSnapshot;
    private DragPreviewForm? _dragPreview;
    private int _columnCount = 1;
    private int _renderRowCount = 1;
    private int _refreshVersion;
    private int _openMaterialCount;
    private int _dragSourceIndex = -1;
    private int _dragTargetIndex = -1;
    private Point _dragStartScreenPosition;
    private Point _dragCursorOffset;
    private bool _resizeSuspended;
    private bool _toolTipsEnabled = true;
    private bool _previousToolTipsEnabled = true;
    private bool _dragPending;
    private bool _dragging;

    public event EventHandler? MaterialStateChanged;
    public event EventHandler? ScrollMetricsChanged;
    public event EventHandler? ScrollPositionChanged;
    public event Action<int>? MouseWheelScrollRequested;
    public event Action<ProjectMaterial>? MaterialRemoveRequested;
    public event Action<int, int>? ReorderRequested;

    public int TotalRows => _materials.Count == 0 ? 0 : (int)Math.Ceiling((double)_materials.Count / _columnCount);
    public int ViewportHeight { get; private set; } = 1;
    public int ScrollStep => MaterialControlHeight + VerticalGap;
    public int ContentHeight => TotalRows == 0 ? 0 : (TotalRows * ScrollStep) - VerticalGap + (ContentPadding * 2);
    public int MaximumScrollOffset => Math.Max(0, ContentHeight - ViewportHeight);
    public int ScrollOffset { get; private set; }

    public VirtualMaterialPanel()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);

        UpdateStyles();
        TabStop = true;

        _dragCancelFilter = new DragCancelFilter(this);
        _dragTimer.Tick += DragTimer_Tick;

        Controls.Add(_dragPlaceholder);
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

        CancelDrag();

        _materials = materials;
        _openMaterialCount = 0;

        foreach(ProjectMaterial material in materials)
        {
            if(material.State is not ProjectMaterialState.Missing and not ProjectMaterialState.Replaced)
                break;

            _openMaterialCount++;
        }

        if(resetScrollPosition)
            ScrollOffset = 0;

        if(_resizeSuspended)
            return;

        RecalculateLayout();
        RefreshVisibleControls();

        ScrollMetricsChanged?.Invoke(this, EventArgs.Empty);
        ScrollPositionChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetScrollOffset(int scrollOffset)
    {
        if(_resizeSuspended)
            return;

        var newScrollOffset = Math.Clamp(scrollOffset, 0, MaximumScrollOffset);

        if(newScrollOffset == ScrollOffset)
            return;

        var oldFirstVisibleRow = ScrollOffset / ScrollStep;
        var newFirstVisibleRow = newScrollOffset / ScrollStep;

        ScrollOffset = newScrollOffset;

        if(oldFirstVisibleRow == newFirstVisibleRow)
        {
            PositionControlPool();
        }
        else if(!TryRecycleRows(oldFirstVisibleRow, newFirstVisibleRow))
        {
            PositionControlPool();
            RefreshVisibleControls();
        }

        ScrollPositionChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetToolTipsEnabled(bool enabled)
    {
        if(enabled && _dragging)
            return;

        if(_toolTipsEnabled == enabled)
            return;

        _toolTipsEnabled = enabled;

        foreach(MaterialControl materialControl in _controlPool)
            materialControl.SetToolTipsEnabled(enabled);
    }

    protected override void OnResize(EventArgs eventArgs)
    {
        base.OnResize(eventArgs);

        if(_dragPending || _dragging)
            CancelDrag();

        if(_itemIconService is null)
            return;

        if(_resizeSuspended)
        {
            Invalidate();
            return;
        }

        var oldColumnCount = _columnCount;
        var oldViewportHeight = ViewportHeight;
        var oldRenderRowCount = _renderRowCount;
        var oldScrollOffset = ScrollOffset;

        RecalculateLayout();

        if(oldColumnCount != _columnCount ||
           oldRenderRowCount != _renderRowCount ||
           oldScrollOffset / ScrollStep != ScrollOffset / ScrollStep)
        {
            RefreshVisibleControls();
        }

        if(oldColumnCount != _columnCount ||
           oldViewportHeight != ViewportHeight ||
           oldScrollOffset != ScrollOffset)
        {
            ScrollMetricsChanged?.Invoke(this, EventArgs.Empty);
            ScrollPositionChanged?.Invoke(this, EventArgs.Empty);
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
        eventArgs.Graphics.DrawImageUnscaled(_resizeSnapshot, Point.Empty);
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
            CancelDrag();

            _dragTimer.Tick -= DragTimer_Tick;
            _dragTimer.Dispose();

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

        _columnCount = Math.Max(1, (availableWidth - (ContentPadding * 2) + HorizontalGap) / (MaterialControlWidth + HorizontalGap));

        ViewportHeight = Math.Max(1, ClientSize.Height);

        _renderRowCount = Math.Max(1, (int)Math.Ceiling((double)ViewportHeight / ScrollStep) + 1);

        ScrollOffset = Math.Min(ScrollOffset, MaximumScrollOffset);

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

            materialControl.SetToolTipsEnabled(_toolTipsEnabled);

            materialControl.StateChanged += MaterialControl_StateChanged;
            materialControl.RemoveRequested += MaterialControl_RemoveRequested;

            RegisterMouseWheel(materialControl);
            RegisterDragSources(materialControl);

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

    private Rectangle GetMaterialBounds(int materialIndex)
    {
        var contentWidth = (_columnCount * MaterialControlWidth) + ((_columnCount - 1) * HorizontalGap);
        var startX = Math.Max(ContentPadding, (ClientSize.Width - contentWidth) / 2);
        var row = materialIndex / _columnCount;
        var column = materialIndex % _columnCount;

        return new Rectangle(startX + (column * (MaterialControlWidth + HorizontalGap)), ContentPadding + (row * ScrollStep) - ScrollOffset, MaterialControlWidth, MaterialControlHeight);
    }

    private void PositionControlPool()
    {
        if(_controlPool.Count == 0)
        {
            PositionDragPlaceholder();
            return;
        }

        var firstMaterialIndex = ScrollOffset / ScrollStep * _columnCount;

        SuspendLayout();

        try
        {
            for(var index = 0; index < _controlPool.Count; index++)
            {
                _controlPool[index].Location = GetMaterialBounds(firstMaterialIndex + index).Location;
                _controlPool[index].Size = new Size(MaterialControlWidth, MaterialControlHeight);
            }
        }
        finally
        {
            ResumeLayout(false);
        }

        PositionDragPlaceholder();
    }

    private bool TryRecycleRows(int oldFirstVisibleRow, int newFirstVisibleRow)
    {
        if(_resizeSuspended)
            return false;

        var rowDelta = newFirstVisibleRow - oldFirstVisibleRow;
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
        var firstMaterialIndex = newFirstVisibleRow * _columnCount;
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
        var firstMaterialIndex = ScrollOffset / ScrollStep * _columnCount;

        List<Task> bindingTasks = [];

        SuspendLayout();

        try
        {
            for(var slotIndex = 0; slotIndex < _controlPool.Count; slotIndex++)
                BindControl(_controlPool[slotIndex], firstMaterialIndex + slotIndex, bindingTasks, forceRebind);
        }
        finally
        {
            ResumeLayout(false);
        }

        if(bindingTasks.Count > 0)
            _ = CompleteRefreshAsync(bindingTasks, refreshVersion);

        PositionDragPlaceholder();
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

        var projectedIndex = GetProjectedIndex(materialIndex);

        if(projectedIndex < 0)
        {
            materialControl.ClearMaterial();
            materialControl.Visible = false;
            return;
        }

        ProjectMaterial material = _materials[projectedIndex];

        materialControl.Visible = true;

        if(!forceRebind &&
           ReferenceEquals(materialControl.BoundMaterial, material))
        {
            return;
        }

        bindingTasks.Add(materialControl.SetMaterialAsync(material));
    }

    private int GetProjectedIndex(int index)
    {
        if(!_dragging || _dragTargetIndex < 0)
            return index;

        if(index == _dragTargetIndex)
            return -1;

        if(_dragSourceIndex < _dragTargetIndex &&
           index >= _dragSourceIndex &&
           index < _dragTargetIndex)
        {
            return index + 1;
        }

        return _dragSourceIndex > _dragTargetIndex &&
           index > _dragTargetIndex &&
           index <= _dragSourceIndex
            ? index - 1
            : index;
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
                Log.Debug(exception, "{LogContext} Ignoring failed material refresh because it is no longer current. RefreshVersion={RefreshVersion}, CurrentRefreshVersion={CurrentRefreshVersion}, ResizeSuspended={ResizeSuspended}", LogContext, refreshVersion, _refreshVersion, _resizeSuspended);

                return;
            }

            Log.Warning(exception, "{LogContext} Failed to refresh visible material controls. RefreshVersion={RefreshVersion}", LogContext, refreshVersion);
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
        if(_resizeSuspended || delta == 0 || MaximumScrollOffset == 0)
            return;

        var wheelSteps = Math.Max(1, Math.Abs(delta) / SystemInformation.MouseWheelScrollDelta);
        var direction = delta > 0 ? -1 : 1;

        MouseWheelScrollRequested?.Invoke(direction * wheelSteps * ScrollStep);
    }

    private void RegisterDragSources(Control control)
    {
        if(control is not Guna.UI2.WinForms.Guna2ImageButton)
            control.MouseDown += DragSource_MouseDown;

        foreach(Control child in control.Controls)
            RegisterDragSources(child);
    }

    private void DragSource_MouseDown(object? sender, MouseEventArgs eventArgs)
    {
        MouseButtons primaryButton = SystemInformation.MouseButtonsSwapped ? MouseButtons.Right : MouseButtons.Left;

        if(eventArgs.Button != primaryButton ||
           _openMaterialCount < 2 ||
           _resizeSuspended ||
           _dragPending ||
           _dragging ||
           sender is not Control control)
        {
            return;
        }

        while(control is not MaterialControl && control.Parent is not null)
            control = control.Parent;

        if(control is not MaterialControl materialControl || materialControl.BoundMaterial is not ProjectMaterial material)
            return;

        if(material.State is not ProjectMaterialState.Missing and not ProjectMaterialState.Replaced)
            return;

        var sourceIndex = -1;

        for(var index = 0; index < _openMaterialCount; index++)
        {
            if(!ReferenceEquals(_materials[index], material))
                continue;

            sourceIndex = index;
            break;
        }

        if(sourceIndex < 0)
            return;

        _dragSourceIndex = sourceIndex;
        _dragTargetIndex = sourceIndex;
        _dragStartScreenPosition = Control.MousePosition;
        _dragCursorOffset = materialControl.PointToClient(_dragStartScreenPosition);
        _dragPending = true;

        _dragTimer.Start();
    }

    private void DragTimer_Tick(object? sender, EventArgs eventArgs)
    {
        Point screenPosition = Control.MousePosition;
        MouseButtons primaryButton = SystemInformation.MouseButtonsSwapped ? MouseButtons.Right : MouseButtons.Left;
        var buttonPressed = (Control.MouseButtons & primaryButton) == primaryButton;

        Form? ownerForm = FindForm();

        if(ownerForm is null || ownerForm.IsDisposed || !ownerForm.ContainsFocus)
        {
            CancelDrag();
            return;
        }

        if(!buttonPressed)
        {
            if(_dragging)
            {
                Point localPosition = PointToClient(screenPosition);
                EndDrag(ClientRectangle.Contains(localPosition));
            }
            else
            {
                CancelDrag();
            }

            return;
        }

        if(_dragPending && !_dragging)
        {
            var distanceX = Math.Abs(screenPosition.X - _dragStartScreenPosition.X);
            var distanceY = Math.Abs(screenPosition.Y - _dragStartScreenPosition.Y);

            if(distanceX < Constants.MaterialDragStartDistancePixels &&
               distanceY < Constants.MaterialDragStartDistancePixels)
            {
                return;
            }

            BeginDrag();
        }

        if(_dragging)
            UpdateDragPosition(screenPosition);
    }

    private void BeginDrag()
    {
        if(!_dragPending || _dragSourceIndex < 0)
            return;

        MaterialControl? sourceControl = null;

        foreach(MaterialControl materialControl in _controlPool)
        {
            if(ReferenceEquals(materialControl.BoundMaterial, _materials[_dragSourceIndex]))
            {
                sourceControl = materialControl;
                break;
            }
        }

        if(sourceControl is null)
        {
            CancelDrag();
            return;
        }

        Bitmap snapshot = new(sourceControl.Width, sourceControl.Height);

        try
        {
            sourceControl.DrawToBitmap(snapshot, new Rectangle(Point.Empty, sourceControl.Size));

            DragPreviewForm preview = new(snapshot)
            {
                Location = new Point(_dragStartScreenPosition.X - _dragCursorOffset.X, _dragStartScreenPosition.Y - _dragCursorOffset.Y)
            };

            Form? ownerForm = FindForm();

            if(ownerForm is null)
            {
                preview.Dispose();
                snapshot.Dispose();
                CancelDrag();
                return;
            }

            _dragPreview = preview;
            _dragPending = false;
            _dragging = true;

            Cursor.Current = Cursors.SizeAll;

            _previousToolTipsEnabled = _toolTipsEnabled;
            SetToolTipsEnabled(false);

            Application.AddMessageFilter(_dragCancelFilter);

            preview.Show(ownerForm);

            RefreshVisibleControls();
        }
        catch
        {
            snapshot.Dispose();
            CancelDrag();
            throw;
        }
    }

    private void UpdateDragPosition(Point screenPosition)
    {
        if(!_dragging)
            return;

        _dragPreview?.Location = new Point(screenPosition.X - _dragCursorOffset.X, screenPosition.Y - _dragCursorOffset.Y);

        Point localPosition = PointToClient(screenPosition);

        if(!ClientRectangle.Contains(localPosition))
        {
            _dragPlaceholder.Visible = false;
            return;
        }

        if(MaximumScrollOffset > 0)
        {
            if(localPosition.Y < Constants.MaterialDragAutoScrollMarginPixels)
                SetScrollOffset(ScrollOffset - Constants.MaterialDragAutoScrollStepPixels);
            else if(localPosition.Y >= ClientSize.Height - Constants.MaterialDragAutoScrollMarginPixels)
                SetScrollOffset(ScrollOffset + Constants.MaterialDragAutoScrollStepPixels);
        }

        var targetIndex = GetDragTargetIndex(localPosition);

        if(targetIndex != _dragTargetIndex)
        {
            _dragTargetIndex = targetIndex;
            RefreshVisibleControls();
        }
        else
        {
            PositionDragPlaceholder();
        }
    }

    private int GetDragTargetIndex(Point localPosition)
    {
        if(_openMaterialCount == 0)
            return -1;

        var contentWidth = (_columnCount * MaterialControlWidth) + ((_columnCount - 1) * HorizontalGap);
        var startX = Math.Max(ContentPadding, (ClientSize.Width - contentWidth) / 2);
        var columnStep = MaterialControlWidth + HorizontalGap;

        var firstColumnCentre = startX + (MaterialControlWidth / 2.0);
        var column = (int)Math.Floor(((localPosition.X - firstColumnCentre) / columnStep) + 0.5);

        var firstRowCentre = ContentPadding + (MaterialControlHeight / 2.0);
        var contentY = localPosition.Y + ScrollOffset;
        var row = (int)Math.Floor(((contentY - firstRowCentre) / ScrollStep) + 0.5);

        row = Math.Clamp(row, 0, Math.Max(0, TotalRows - 1));

        var firstIndexInRow = row * _columnCount;
        var availableColumns = Math.Min(_columnCount, _materials.Count - firstIndexInRow);

        column = Math.Clamp(column, 0, Math.Max(0, availableColumns - 1));

        var targetIndex = firstIndexInRow + column;

        return targetIndex < _openMaterialCount ? targetIndex : -1;
    }

    private void PositionDragPlaceholder()
    {
        if(!_dragging ||
           _dragTargetIndex < 0 ||
           _dragTargetIndex >= _openMaterialCount)
        {
            _dragPlaceholder.Visible = false;
            return;
        }

        Rectangle targetBounds = GetMaterialBounds(_dragTargetIndex);

        _dragPlaceholder.Bounds = targetBounds;
        _dragPlaceholder.Visible = targetBounds.Bottom > 0 &&
                                   targetBounds.Top < ClientSize.Height &&
                                   targetBounds.Right > 0 &&
                                   targetBounds.Left < ClientSize.Width;

        if(_dragPlaceholder.Visible)
            _dragPlaceholder.BringToFront();
    }

    private void CancelDrag()
    {
        EndDrag(false);
    }

    private void EndDrag(bool commit)
    {
        if(!_dragPending && !_dragging)
            return;

        var shouldReorder = commit &&
                            _dragging &&
                            _dragSourceIndex >= 0 &&
                            _dragTargetIndex >= 0 &&
                            _dragTargetIndex < _openMaterialCount &&
                            _dragSourceIndex != _dragTargetIndex;

        var sourceIndex = _dragSourceIndex;
        var targetIndex = _dragTargetIndex;
        var wasDragging = _dragging;

        _dragTimer.Stop();

        if(wasDragging)
            Application.RemoveMessageFilter(_dragCancelFilter);

        _dragPending = false;
        _dragging = false;
        _dragSourceIndex = -1;
        _dragTargetIndex = -1;

        Cursor.Current = Cursors.Default;

        _dragPlaceholder.Visible = false;

        if(_dragPreview is not null)
        {
            Image? image = _dragPreview.BackgroundImage;
            _dragPreview.BackgroundImage = null;
            _dragPreview.Dispose();
            image?.Dispose();
            _dragPreview = null;
        }

        if(wasDragging)
        {
            SetToolTipsEnabled(_previousToolTipsEnabled);

            if(!IsDisposed && !Disposing && !_resizeSuspended)
                RefreshVisibleControls();
        }

        if(shouldReorder && !IsDisposed && !Disposing)
            ReorderRequested?.Invoke(sourceIndex, targetIndex);
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

        CancelDrag();
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

        Log.Debug("{LogContext} Virtual layout suspended with resize snapshot. ControlCount={ControlCount}, RefreshVersion={RefreshVersion}, SnapshotAvailable={SnapshotAvailable}", LogContext, _controlPool.Count, _refreshVersion, _resizeSnapshot is not null);
    }

    public void ResumeVirtualLayout()
    {
        if(!_resizeSuspended)
            return;

        _resizeSuspended = false;

        RecalculateLayout();
        RefreshVisibleControls(forceRebind: true);

        _resizeSnapshot?.Dispose();
        _resizeSnapshot = null;

        ScrollMetricsChanged?.Invoke(this, EventArgs.Empty);
        ScrollPositionChanged?.Invoke(this, EventArgs.Empty);

        Invalidate();

        Log.Debug("{LogContext} Virtual layout resumed after snapshot resize. ColumnCount={ColumnCount}, ViewportHeight={ViewportHeight}, RenderRowCount={RenderRowCount}, ScrollOffset={ScrollOffset}", LogContext, _columnCount, ViewportHeight, _renderRowCount, ScrollOffset);
    }

    private sealed class DragCancelFilter(VirtualMaterialPanel owner) : IMessageFilter
    {
        private readonly VirtualMaterialPanel _owner = owner;

        public bool PreFilterMessage(ref Message message)
        {
            const int WmKeyDown = 0x0100;

            if(message.Msg == WmKeyDown && message.WParam.ToInt32() == (int)Keys.Escape)
            {
                _owner.CancelDrag();
                return true;
            }

            return false;
        }
    }

    private sealed class DragPreviewForm : Form
    {
        protected override bool ShowWithoutActivation => true;

        protected override CreateParams CreateParams
        {
            get
            {
                const int WsExTransparent = 0x00000020;
                const int WsExToolWindow = 0x00000080;
                const int WsExNoActivate = 0x08000000;

                CreateParams parameters = base.CreateParams;
                parameters.ExStyle |= WsExTransparent | WsExToolWindow | WsExNoActivate;
                return parameters;
            }
        }

        public DragPreviewForm(Bitmap snapshot)
        {
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            Size = snapshot.Size;
            BackgroundImage = snapshot;
            BackgroundImageLayout = ImageLayout.None;
            Opacity = Constants.MaterialDragPreviewOpacity;
        }
    }

    private sealed class DragPlaceholder : Control
    {
        public DragPlaceholder()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);

            BackColor = Color.Transparent;
            Visible = false;
            TabStop = false;
        }

        protected override void OnPaint(PaintEventArgs eventArgs)
        {
            base.OnPaint(eventArgs);

            using SolidBrush backgroundBrush = new(Constants.MaterialDragPlaceholderBackgroundColor);
            using Pen borderPen = new(Constants.MaterialDragPlaceholderColor, 2f) { DashStyle = DashStyle.Dash };

            eventArgs.Graphics.FillRectangle(backgroundBrush, ClientRectangle);
            eventArgs.Graphics.DrawRectangle(borderPen, 2, 2, Math.Max(0, Width - 5), Math.Max(0, Height - 5));

            TextRenderer.DrawText(eventArgs.Graphics, "Move to here", Font, ClientRectangle, Constants.MaterialDragPlaceholderColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);
        }
    }
}