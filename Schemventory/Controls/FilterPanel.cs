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
            TTP_Main.SetToolTip(CHP_Amount, $"{field} {FilterStyle.ToString().ToLower()} materials");
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

        CHB_Value.CheckedState.FillColor = style.BorderColor;
        CHB_Value.Text = style.Text;
        TTP_Main.SetToolTip(CHB_Value, $"Filter for {FilterStyle.ToString().ToLower()} materials");

        if(Checked)
        {
            PNL_Background.BorderColor = style.BorderColor;
            PNL_Background.FillColor = style.BackgroundColor;

            CHP_Amount.BorderColor = style.BorderColor;
            CHP_Amount.FillColor = style.BackgroundColor;
        }
        else
        {
            PNL_Background.BorderColor = Color.FromArgb(103, 99, 99);
            PNL_Background.FillColor = Color.FromArgb(58, 55, 55);

            CHP_Amount.BorderColor = Color.FromArgb(103, 99, 99);
            CHP_Amount.FillColor = Color.FromArgb(80, 76, 76);
        }
    }

    private void CHB_Value_CheckedChanged(object sender, EventArgs e)
    {
        ApplyStyle();
        CheckedChanged?.Invoke(this, EventArgs.Empty);
    }

    private static FilterStyleDefinition GetStyleDefinition(FilterStyle filterStyle)
    {
        return filterStyle switch
        {
            FilterStyle.Missing => new(
                "Missing",
                Color.FromArgb(217, 74, 74),
                Color.FromArgb(65, 40, 40)),

            FilterStyle.Replaced => new(
                "Replaced",
                Color.FromArgb(43, 133, 187),
                Color.FromArgb(39, 49, 58)),

            FilterStyle.Collected => new(
                "Collected",
                Color.FromArgb(71, 213, 166),
                Color.FromArgb(43, 61, 54)),

            FilterStyle.Ignored => new(
                "Ignored",
                Color.FromArgb(215, 172, 97),
                Color.FromArgb(65, 56, 43)),

            _ => throw new ArgumentOutOfRangeException(nameof(filterStyle), filterStyle, null)
        };
    }

    private sealed record FilterStyleDefinition(string Text, Color BorderColor, Color BackgroundColor);
}

public enum FilterStyle
{
    Missing,
    Replaced,
    Collected,
    Ignored
}