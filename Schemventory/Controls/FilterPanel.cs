using System.ComponentModel;

namespace Schemventory.Controls;

public partial class FilterPanel : UserControl
{
    [Category("Appearance")]
    [DefaultValue(0)]
    public int Count
    {
        get;
        set
        {
            field = Math.Max(0, value);
            CHP_Amount.Text = field.ToString();
        }
    }

    [Category("Behavior")]
    [DefaultValue(false)]
    public bool Checked
    {
        get => CHB_Value.Checked;
        set => CHB_Value.Checked = value;
    }

    [Category("Appearance")]
    [DefaultValue(FilterStyle.Missing)]
    public FilterStyle FilterStyle
    {
        get;
        set
        {
            if(field == value)
                return;

            field = value;
            ApplyStyle();
        }
    } = FilterStyle.Missing;

    [Browsable(false)]
    public string FilterText => CHB_Value.Text;

    [Category("Action")]
    public event EventHandler? CheckedChanged;

    public FilterPanel()
    {
        InitializeComponent();
        ApplyStyle();
    }

    private void ApplyStyle()
    {
        FilterStyleDefinition style = GetStyleDefinition(FilterStyle);

        PNL_Background.BorderColor = style.BorderColor;
        PNL_Background.FillColor = style.BackgroundColor;

        CHB_Value.CheckedState.FillColor = style.CheckboxColor;
        CHB_Value.Text = style.Text;

        CHP_Amount.FillColor = style.ChipColor;
    }

    private void CHB_Value_CheckedChanged(object sender, EventArgs e)
    {
        CheckedChanged?.Invoke(this, EventArgs.Empty);
    }

    private static FilterStyleDefinition GetStyleDefinition(FilterStyle filterStyle)
    {
        return filterStyle switch
        {
            FilterStyle.Missing => new(
                "Missing",
                Color.FromArgb(217, 74, 74),
                Color.FromArgb(64, 47, 48),
                Color.FromArgb(176, 78, 77),
                Color.FromArgb(217, 74, 74)),

            FilterStyle.Replaced => new(
                "Replaced",
                Color.FromArgb(43, 133, 187),
                Color.FromArgb(45, 53, 60),
                Color.FromArgb(35, 107, 150),
                Color.FromArgb(43, 133, 187)),

            FilterStyle.Collected => new(
                "Collected",
                Color.FromArgb(71, 213, 166),
                Color.FromArgb(47, 50, 46),
                Color.FromArgb(73, 104, 66),
                Color.FromArgb(71, 213, 166)),

            FilterStyle.Ignored => new(
                "Ignored",
                Color.FromArgb(215, 172, 97),
                Color.FromArgb(62, 56, 47),
                Color.FromArgb(190, 152, 67),
                Color.FromArgb(215, 172, 97)),

            _ => throw new ArgumentOutOfRangeException(nameof(filterStyle), filterStyle, null)
        };
    }

    private sealed record FilterStyleDefinition(string Text, Color BorderColor, Color BackgroundColor, Color ChipColor, Color CheckboxColor);
}

public enum FilterStyle
{
    Missing,
    Replaced,
    Collected,
    Ignored
}