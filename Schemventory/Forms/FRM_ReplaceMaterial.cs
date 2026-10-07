using Schemventory.App;
using Schemventory.Data;
using Schemventory.Services;

namespace Schemventory.Forms;

public partial class FRM_ReplaceMaterial : Form
{
    private const string WindowName = $"Replace material - {Constants.AppName}";
    private readonly ProjectMaterial _material;
    private readonly ItemDataProvider _itemDataProvider;
    private readonly ItemIconService _itemIconService;
    private List<ItemData> _items = [];
    private List<ItemData> _filteredItems = [];
    private CancellationTokenSource? _previewCancellationTokenSource;
    public ItemData? SelectedItem { get; private set; }
    public string? ReplacementItemId => SelectedItem?.Id;
    public bool ResetRequested { get; private set; }

    public FRM_ReplaceMaterial(ProjectMaterial material, ItemDataProvider itemDataProvider, ItemIconService itemIconService)
    {
        InitializeComponent();

        _material = material;
        _itemDataProvider = itemDataProvider;
        _itemIconService = itemIconService;

        Text = WindowName;

        InitializeListView();
        InitializeForm();
        RegisterEvents();
    }

    private void InitializeForm()
    {
        LBL_Description.Text = $"Choose a material to replace {GetDisplayName(_material.ItemId)} with.";

        LBL_ItemName.Text = "No material selected";
        LBL_ItemId.Text = string.Empty;
        LBL_StackSize.Text = string.Empty;

        PBX_Icon.Image = null;

        BTN_Replace.Enabled = false;

        BTN_Reset.Visible = !string.IsNullOrWhiteSpace(_material.ReplacementItemId);
    }

    private void InitializeListView()
    {
        LST_Items.Columns.Clear();

        _ = LST_Items.Columns.Add("Material", 180);
        _ = LST_Items.Columns.Add("Item ID", 250);
    }

    private void RegisterEvents()
    {
        Load += FRM_ReplaceMaterial_Load;

        TBX_Search.TextChanged += TBX_Search_TextChanged;

        LST_Items.RetrieveVirtualItem += LST_Items_RetrieveVirtualItem;
        LST_Items.SelectedIndexChanged += LST_Items_SelectedIndexChanged;
        LST_Items.DoubleClick += LST_Items_DoubleClick;

        BTN_Replace.Click += BTN_Replace_Click;
        BTN_Cancel.Click += BTN_Cancel_Click;

        FormClosed += FRM_ReplaceMaterial_FormClosed;
    }

    private async void FRM_ReplaceMaterial_Load(object? sender, EventArgs e)
    {
        try
        {
            BTN_Replace.Enabled = false;
            TBX_Search.Enabled = false;
            LST_Items.Enabled = false;

            IReadOnlyCollection<ItemData> items =
                await _itemDataProvider.GetItemsAsync();

            _items = items
                .Where(x => !string.Equals(
                    x.Id,
                    NormalizeItemId(_material.ItemId),
                    StringComparison.Ordinal))
                .OrderBy(x => GetDisplayName(x.Id), StringComparer.CurrentCultureIgnoreCase)
                .ToList();

            _filteredItems = [.. _items];

            UpdateVirtualList();

            TBX_Search.Enabled = true;
            LST_Items.Enabled = true;

            TBX_Search.Focus();
        }
        catch(Exception exception)
        {
            _ = MessageBox.Show(
                $"The material list could not be loaded.\n\n{exception.Message}",
                Constants.AppName,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void TBX_Search_TextChanged(object? sender, EventArgs e)
    {
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        var searchText = TBX_Search.Text.Trim();

        _filteredItems = string.IsNullOrWhiteSpace(searchText)
            ? [.. _items]
            : _items
                .Where(x =>
                    x.Id.Contains(
                        searchText,
                        StringComparison.OrdinalIgnoreCase) ||
                    GetDisplayName(x.Id).Contains(
                        searchText,
                        StringComparison.CurrentCultureIgnoreCase))
                .ToList();

        SelectedItem = null;

        ClearPreview();
        UpdateVirtualList();
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
    }

    private void LST_Items_RetrieveVirtualItem(object? sender, RetrieveVirtualItemEventArgs e)
    {
        if(e.ItemIndex < 0 || e.ItemIndex >= _filteredItems.Count)
        {
            e.Item = new ListViewItem();
            return;
        }

        ItemData item = _filteredItems[e.ItemIndex];

        ListViewItem listViewItem = new(GetDisplayName(item.Id));

        _ = listViewItem.SubItems.Add(GetFullItemId(item.Id));

        e.Item = listViewItem;
    }

    private async void LST_Items_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if(LST_Items.SelectedIndices.Count == 0)
        {
            SelectedItem = null;
            ClearPreview();
            return;
        }

        var selectedIndex = LST_Items.SelectedIndices[0];

        if(selectedIndex < 0 || selectedIndex >= _filteredItems.Count)
        {
            SelectedItem = null;
            ClearPreview();
            return;
        }

        SelectedItem = _filteredItems[selectedIndex];

        BTN_Replace.Enabled = true;

        LBL_ItemName.Text = GetDisplayName(SelectedItem.Id);
        LBL_ItemId.Text = GetFullItemId(SelectedItem.Id);
        LBL_StackSize.Text = $"Stack size: {SelectedItem.MaxStackSize}";

        await LoadPreviewIconAsync(SelectedItem.Id);
    }

    private async Task LoadPreviewIconAsync(string itemId)
    {
        _previewCancellationTokenSource?.Cancel();
        _previewCancellationTokenSource?.Dispose();

        _previewCancellationTokenSource = new CancellationTokenSource();

        CancellationToken cancellationToken =
            _previewCancellationTokenSource.Token;

        try
        {
            Image image = await _itemIconService.GetItemIconAsync(
                itemId,
                64,
                cancellationToken);

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
        }
        catch
        {
            if(IsDisposed)
                return;

            PBX_Icon.Image?.Dispose();
            PBX_Icon.Image = new Bitmap(Properties.Resources.MissingIcon);
            PBX_Icon.SizeMode = PictureBoxSizeMode.Zoom;
        }
    }

    private void ClearPreview()
    {
        _previewCancellationTokenSource?.Cancel();

        SelectedItem = null;

        BTN_Replace.Enabled = false;

        LBL_ItemName.Text = "No material selected";
        LBL_ItemId.Text = string.Empty;
        LBL_StackSize.Text = string.Empty;

        PBX_Icon.Image?.Dispose();
        PBX_Icon.Image = null;
    }

    private void LST_Items_DoubleClick(object? sender, EventArgs e)
    {
        ConfirmSelection();
    }

    private void BTN_Replace_Click(object? sender, EventArgs e)
    {
        ConfirmSelection();
    }

    private void BTN_Reset_Click(object? sender, EventArgs e)
    {
        ResetRequested = true;
        SelectedItem = null;

        DialogResult = DialogResult.OK;
        Close();
    }

    private void ConfirmSelection()
    {
        if(SelectedItem is null)
            return;

        DialogResult = DialogResult.OK;
        Close();
    }

    private void BTN_Cancel_Click(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }

    private void FRM_ReplaceMaterial_FormClosed(object? sender, FormClosedEventArgs e)
    {
        _previewCancellationTokenSource?.Cancel();
        _previewCancellationTokenSource?.Dispose();

        PBX_Icon.Image?.Dispose();
        PBX_Icon.Image = null;
    }

    private static string NormalizeItemId(string itemId)
    {
        const string prefix = "minecraft:";

        return itemId.StartsWith(prefix, StringComparison.Ordinal)
            ? itemId[prefix.Length..]
            : itemId;
    }

    private static string GetFullItemId(string itemId)
    {
        return itemId.StartsWith("minecraft:", StringComparison.Ordinal)
            ? itemId
            : $"minecraft:{itemId}";
    }

    private static string GetDisplayName(string itemId)
    {
        var name = NormalizeItemId(itemId);

        name = name.Replace('_', ' ');

        return System.Globalization.CultureInfo.CurrentCulture
            .TextInfo
            .ToTitleCase(name);
    }
}