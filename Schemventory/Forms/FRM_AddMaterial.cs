using Schemventory.App;
using Schemventory.Data;
using Schemventory.Services;
using Serilog;

namespace Schemventory.Forms;

public partial class FRM_AddMaterial : Form
{
    private const string LogContext = "(FRM_AddMaterial)";
    private const int MinimumThumbHeight = 24;
    private const string WindowName = $"Add material - {Constants.AppName}";

    private readonly ItemDataProvider _itemDataProvider;
    private readonly ItemIconService _itemIconService;
    private readonly System.Windows.Forms.Timer _searchTimer;


    private List<ItemSearchEntry> _items = [];
    private List<ItemSearchEntry> _filteredItems = [];
    private CancellationTokenSource? _previewCancellationTokenSource;

    public ItemData? SelectedItem { get; private set; }

    public string? SelectedItemId => SelectedItem is null ? null : GetFullItemId(SelectedItem.Id);

    public long RequiredAmount => (long)NUD_RequiredAmount.Value;

    public FRM_AddMaterial(ItemDataProvider itemDataProvider, ItemIconService itemIconService)
    {
        InitializeComponent();

        _itemDataProvider = itemDataProvider;
        _itemIconService = itemIconService;

        _searchTimer = new System.Windows.Forms.Timer
        {
            Interval = 150
        };

        _searchTimer.Tick += SearchTimer_Tick;


        Text = WindowName;
        Icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath);

        InitializeListView();
        InitializeForm();

        LST_Items.RetrieveVirtualItem += LST_Items_RetrieveVirtualItem;
        LST_Items.SizeChanged += LST_Items_SizeChanged;
        FormClosed += FRM_AddMaterial_FormClosed;
    }

    private void InitializeForm()
    {
        LBL_ItemName.Text = "No material selected";
        LBL_ItemId.Text = string.Empty;
        LBL_StackSize.Text = string.Empty;
        LBL_AmountPreview.Text = string.Empty;

        PBX_Icon.Image = null;

        NUD_RequiredAmount.Value = 1;
        NUD_RequiredAmount.Enabled = false;

        BTN_Add.Enabled = false;
    }

    private void InitializeListView()
    {
        LST_Items.Sorting = SortOrder.None;
        LST_Items.Columns.Clear();

        _ = LST_Items.Columns.Add("Material", 280);
        _ = LST_Items.Columns.Add("Item ID", 250);
    }

    private async void FRM_AddMaterial_Load(object? sender, EventArgs e)
    {
        Log.Debug("{LogContext} Add material dialog opened.", LogContext);

        try
        {
            BTN_Add.Enabled = false;
            NUD_RequiredAmount.Enabled = false;
            TBX_Search.Enabled = false;
            LST_Items.Enabled = false;

            IReadOnlyCollection<ItemData> items = await _itemDataProvider.GetItemsAsync();

            _items = items
                .Select(x => new ItemSearchEntry(x, GetDisplayName(x.Id), GetFullItemId(x.Id)))
                .OrderBy(x => x.DisplayName, StringComparer.CurrentCultureIgnoreCase)
                .ToList();

            _filteredItems = [.. _items];

            UpdateVirtualList();

            TBX_Search.Enabled = true;
            LST_Items.Enabled = true;
            TBX_Search.Focus();

            Log.Debug("{LogContext} Materials loaded. Count={Count}", LogContext, _items.Count);
        }
        catch(Exception exception)
        {
            Log.Error(exception, "{LogContext} Failed to load material list.", LogContext);

            _ = MessageBox.Show($"The material list could not be loaded.\n\n{exception.Message}", Constants.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void TBX_Search_TextChanged(object? sender, EventArgs e)
    {
        _searchTimer.Stop();
        _searchTimer.Start();
    }

    private void SearchTimer_Tick(object? sender, EventArgs e)
    {
        _searchTimer.Stop();
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        var searchText = TBX_Search.Text.Trim();

        _filteredItems = string.IsNullOrWhiteSpace(searchText)
            ? [.. _items]
            : _items.Where(x => x.Item.Id.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                                x.FullItemId.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                                x.DisplayName.Contains(searchText, StringComparison.CurrentCultureIgnoreCase))
                .ToList();

        ClearPreview();
        UpdateVirtualList();

        Log.Debug("{LogContext} Material search applied. Search={Search}, ResultCount={ResultCount}", LogContext, searchText, _filteredItems.Count);
    }

    private void UpdateVirtualList()
    {
        LST_Items.BeginUpdate();

        try
        {
            LST_Items.VirtualListSize = _filteredItems.Count;
            LST_Items.SelectedIndices.Clear();
        }
        finally
        {
            LST_Items.EndUpdate();
        }

        LST_Items.Invalidate();

        UpdateScrollBarThumbSize();
    }

    private void LST_Items_RetrieveVirtualItem(object? sender, RetrieveVirtualItemEventArgs e)
    {
        if(e.ItemIndex < 0 || e.ItemIndex >= _filteredItems.Count)
        {
            e.Item = new ListViewItem();
            return;
        }

        ItemSearchEntry entry = _filteredItems[e.ItemIndex];
        ListViewItem listViewItem = new(entry.DisplayName);

        _ = listViewItem.SubItems.Add(entry.FullItemId);

        e.Item = listViewItem;
    }

    private async void LST_Items_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if(LST_Items.SelectedIndices.Count == 0)
        {
            ClearPreview();
            return;
        }

        var selectedIndex = LST_Items.SelectedIndices[0];

        if(selectedIndex < 0 || selectedIndex >= _filteredItems.Count)
        {
            ClearPreview();
            return;
        }

        ItemSearchEntry entry = _filteredItems[selectedIndex];


        SelectedItem = entry.Item;

        NUD_RequiredAmount.Enabled = true;
        BTN_Add.Enabled = true;

        LBL_ItemName.Text = entry.DisplayName;
        LBL_ItemId.Text = entry.FullItemId;
        LBL_StackSize.Text = $"Stack size: {entry.Item.MaxStackSize}";

        UpdateAmountPreview();

        Log.Debug("{LogContext} Material selected. ItemId={ItemId}", LogContext, entry.FullItemId);

        await LoadPreviewIconAsync(entry.Item.Id);
    }

    private async Task LoadPreviewIconAsync(string itemId)
    {
        _previewCancellationTokenSource?.Cancel();
        _previewCancellationTokenSource?.Dispose();

        _previewCancellationTokenSource = new CancellationTokenSource();

        CancellationToken cancellationToken = _previewCancellationTokenSource.Token;

        try
        {
            Image image = await _itemIconService.GetItemIconAsync(itemId, 64, cancellationToken);

            if(cancellationToken.IsCancellationRequested || IsDisposed)
            {
                image.Dispose();
                return;
            }

            PBX_Icon.Image?.Dispose();
            PBX_Icon.Image = image;
            PBX_Icon.SizeMode = PictureBoxSizeMode.Zoom;
        }
        catch(OperationCanceledException)
        {
            Log.Debug("{LogContext} Preview icon loading cancelled. ItemId={ItemId}", LogContext, itemId);
        }
        catch(Exception exception)
        {
            Log.Warning(exception, "{LogContext} Failed to load preview icon. ItemId={ItemId}", LogContext, itemId);

            if(IsDisposed)
                return;

            PBX_Icon.Image?.Dispose();
            PBX_Icon.Image = new Bitmap(Properties.Resources.MissingIcon);
            PBX_Icon.SizeMode = PictureBoxSizeMode.Zoom;
        }
    }

    private void NUD_RequiredAmount_ValueChanged(object? sender, EventArgs e)
    {
        UpdateAmountPreview();
    }

    private void UpdateAmountPreview()
    {
        if(SelectedItem is null)
        {
            LBL_AmountPreview.Text = string.Empty;
            return;
        }

        var amount = (long)NUD_RequiredAmount.Value;
        var stackSize = SelectedItem.MaxStackSize;
        var stacks = amount / stackSize;
        var blocks = amount % stackSize;

        var stackText = stacks == 1 ? "Stack" : "Stacks";
        var blockText = blocks == 1 ? "Block" : "Blocks";

        LBL_AmountPreview.Text = stacks > 0 && blocks > 0
            ? $"{stacks} {stackText} + {blocks} {blockText}"
            : stacks > 0
                ? $"{stacks} {stackText}"
                : $"{blocks} {blockText}";
    }

    private void ClearPreview()
    {
        _previewCancellationTokenSource?.Cancel();

        SelectedItem = null;

        BTN_Add.Enabled = false;
        NUD_RequiredAmount.Enabled = false;
        NUD_RequiredAmount.Value = 1;

        LBL_ItemName.Text = "No material selected";
        LBL_ItemId.Text = string.Empty;
        LBL_StackSize.Text = string.Empty;
        LBL_AmountPreview.Text = string.Empty;

        PBX_Icon.Image?.Dispose();
        PBX_Icon.Image = null;
    }

    private void LST_Items_DoubleClick(object? sender, EventArgs e)
    {
        ConfirmSelection();
    }

    private void BTN_Add_Click(object? sender, EventArgs e)
    {
        ConfirmSelection();
    }

    private void ConfirmSelection()
    {
        if(SelectedItem is null || RequiredAmount <= 0)
            return;

        Log.Debug("{LogContext} Material addition confirmed. ItemId={ItemId}, RequiredAmount={RequiredAmount}", LogContext, SelectedItemId, RequiredAmount);

        DialogResult = DialogResult.OK;
        Close();
    }

    private void BTN_Cancel_Click(object? sender, EventArgs e)
    {
        Log.Debug("{LogContext} Add material dialog cancelled.", LogContext);

        DialogResult = DialogResult.Cancel;
        Close();
    }

    private void LST_Items_SizeChanged(object? sender, EventArgs e)
    {
        UpdateScrollBarThumbSize();
    }

    private void UpdateScrollBarThumbSize()
    {
        var totalItems = _filteredItems.Count;

        if(totalItems <= 0 || LST_Items.ClientSize.Height <= 0 || VSB_Main.ClientSize.Height <= 0)
            return;

        var itemHeight = GetListViewItemHeight();
        var visibleItems = Math.Max(1, LST_Items.ClientSize.Height / itemHeight);

        var proportionalThumbSize = VSB_Main.ClientSize.Height * Math.Min(1F, (float)visibleItems / totalItems);
        var maximumThumbSize = Math.Max(1F, VSB_Main.ClientSize.Height);

        VSB_Main.ThumbSize = Math.Clamp(proportionalThumbSize, MinimumThumbHeight, maximumThumbSize);
    }

    private int GetListViewItemHeight()
    {
        if(LST_Items.VirtualListSize > 0)
        {
            try
            {
                var itemHeight = LST_Items.Items[0].Bounds.Height;

                if(itemHeight > 0)
                    return itemHeight;
            }
            catch(InvalidOperationException)
            {
                // The ListView handle may not be ready yet.
            }
        }

        return Math.Max(1, LST_Items.Font.Height + 4);
    }

    private void FRM_AddMaterial_FormClosed(object? sender, FormClosedEventArgs e)
    {
        LST_Items.SizeChanged -= LST_Items_SizeChanged;


        _searchTimer.Stop();
        _searchTimer.Dispose();

        _previewCancellationTokenSource?.Cancel();
        _previewCancellationTokenSource?.Dispose();

        PBX_Icon.Image?.Dispose();
        PBX_Icon.Image = null;
    }

    private static string NormalizeItemId(string itemId)
    {
        const string prefix = "minecraft:";

        return itemId.StartsWith(prefix, StringComparison.Ordinal) ? itemId[prefix.Length..] : itemId;
    }

    private static string GetFullItemId(string itemId)
    {
        return itemId.StartsWith("minecraft:", StringComparison.Ordinal) ? itemId : $"minecraft:{itemId}";
    }

    private static string GetDisplayName(string itemId)
    {
        var name = NormalizeItemId(itemId);

        name = name.Replace('_', ' ');

        return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name);
    }

    private sealed record ItemSearchEntry(ItemData Item, string DisplayName, string FullItemId);
}
