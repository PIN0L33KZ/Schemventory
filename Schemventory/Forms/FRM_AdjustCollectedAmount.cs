using Guna.UI2.WinForms;
using Serilog;
using Schemventory.App;

namespace Schemventory.Forms;

public partial class FRM_AdjustCollectedAmount : Form
{
    private const string LogContext = "(FRM_AdjustCollectedAmount)";
    private const string WindowName = $"Adjust collected amount - {Constants.AppName}";

    private enum AmountDirection
    {
        None,
        Add,
        Remove
    }

    private readonly ProjectMaterial _material;
    private AmountDirection _direction = AmountDirection.None;
    private bool _updatingValues;

    public long AmountChange { get; private set; }

    public FRM_AdjustCollectedAmount(ProjectMaterial material)
    {
        InitializeComponent();

        _material = material;

        Text = WindowName;

        InitializeValues();

        Log.Debug("{LogContext} Adjust collected amount dialogue opened. ProjectId={ProjectId}, ItemId={ItemId}, CollectedAmount={CollectedAmount}, RequiredAmount={RequiredAmount}", LogContext, _material.ProjectId, _material.ItemId, _material.CollectedAmount, _material.RequiredAmount);
    }

    private void InitializeValues()
    {
        _updatingValues = true;

        NUD_Stacks.Value = 0;
        NUD_Blocks.Value = 0;

        _updatingValues = false;
        _direction = AmountDirection.None;

        UpdateLimits();
        UpdateHeading();
        UpdateCollectedStats();
    }

    private void NUD_Stacks_ValueChanged(object sender, EventArgs e)
    {
        HandleValueChanged(NUD_Stacks);
    }

    private void NUD_Blocks_ValueChanged(object sender, EventArgs e)
    {
        HandleValueChanged(NUD_Blocks);
    }

    private void HandleValueChanged(Guna2NumericUpDown changedControl)
    {
        if(_updatingValues)
            return;

        if(changedControl.Value > 0)
            _direction = AmountDirection.Add;
        else if(changedControl.Value < 0)
            _direction = AmountDirection.Remove;
        else if(NUD_Stacks.Value == 0 && NUD_Blocks.Value == 0)
            _direction = AmountDirection.None;

        _updatingValues = true;

        try
        {
            NormalizeDirection();
            UpdateLimits();
            UpdateHeading();
        }
        finally
        {
            _updatingValues = false;
        }
    }

    private void NormalizeDirection()
    {
        switch(_direction)
        {
            case AmountDirection.Add:
                if(NUD_Stacks.Value < 0)
                    NUD_Stacks.Value = 0;

                if(NUD_Blocks.Value < 0)
                    NUD_Blocks.Value = 0;
                break;

            case AmountDirection.Remove:
                if(NUD_Stacks.Value > 0)
                    NUD_Stacks.Value = 0;

                if(NUD_Blocks.Value > 0)
                    NUD_Blocks.Value = 0;
                break;
        }
    }

    private void UpdateLimits()
    {
        long stackSize = _material.MaxStackSize;
        var maxAddAmount = _material.RemainingAmount;
        var maxRemoveAmount = _material.CollectedAmount;

        var maxAddStacks = maxAddAmount / stackSize;
        var maxRemoveStacks = maxRemoveAmount / stackSize;

        switch(_direction)
        {
            case AmountDirection.Add:
                SetRange(NUD_Stacks, 0, maxAddStacks);
                UpdateAddBlockLimits(stackSize, maxAddAmount);
                break;

            case AmountDirection.Remove:
                SetRange(NUD_Stacks, -maxRemoveStacks, 0);
                UpdateRemoveBlockLimits(stackSize, maxRemoveAmount);
                break;

            default:
                SetRange(NUD_Stacks, -maxRemoveStacks, maxAddStacks);

                var maxAddBlocks = Math.Min(stackSize - 1, maxAddAmount);
                var maxRemoveBlocks = Math.Min(stackSize - 1, maxRemoveAmount);

                SetRange(NUD_Blocks, -maxRemoveBlocks, maxAddBlocks);
                break;
        }

        BTN_Save.Enabled = NUD_Stacks.Value != 0 || NUD_Blocks.Value != 0;
    }

    private void UpdateAddBlockLimits(long stackSize, long maxAddAmount)
    {
        var stackAmount = (long)NUD_Stacks.Value * stackSize;
        var remainingAmount = Math.Max(0, maxAddAmount - stackAmount);
        var maxBlocks = Math.Min(stackSize - 1, remainingAmount);

        SetRange(NUD_Blocks, 0, maxBlocks);
    }

    private void UpdateRemoveBlockLimits(long stackSize, long maxRemoveAmount)
    {
        var stackAmount = Math.Abs((long)NUD_Stacks.Value) * stackSize;
        var remainingAmount = Math.Max(0, maxRemoveAmount - stackAmount);
        var maxBlocks = Math.Min(stackSize - 1, remainingAmount);

        SetRange(NUD_Blocks, -maxBlocks, 0);
    }

    private static void SetRange(Guna2NumericUpDown numericUpDown, decimal minimum, decimal maximum)
    {
        var value = Math.Clamp(numericUpDown.Value, minimum, maximum);

        if(minimum < numericUpDown.Minimum)
            numericUpDown.Minimum = minimum;

        if(maximum > numericUpDown.Maximum)
            numericUpDown.Maximum = maximum;

        numericUpDown.Value = value;
        numericUpDown.Minimum = minimum;
        numericUpDown.Maximum = maximum;
    }

    private void UpdateHeading()
    {
        LBL_Heading.Text = _direction switch
        {
            AmountDirection.Add => "Add to collected",
            AmountDirection.Remove => "Remove from collected",
            _ => "Adjust collected amount"
        };
    }

    private void UpdateCollectedStats()
    {
        var collectedAmount = _material.CollectedAmount;
        var stacks = collectedAmount / _material.MaxStackSize;
        var blocks = collectedAmount % _material.MaxStackSize;

        if(stacks == 0 && blocks == 0)
        {
            LBL_CollectedStats.Text = "Already collected: 0 Blocks";
            return;
        }

        var stackText = stacks == 1 ? "Stack" : "Stacks";
        var blockText = blocks == 1 ? "Block" : "Blocks";

        LBL_CollectedStats.Text = stacks > 0 && blocks > 0
            ? $"Already collected: {stacks} {stackText} + {blocks} {blockText}"
            : stacks > 0
                ? $"Already collected: {stacks} {stackText}"
                : $"Already collected: {blocks} {blockText}";
    }

    private void BTN_Save_Click(object sender, EventArgs e)
    {
        var stacks = (long)NUD_Stacks.Value;
        var blocks = (long)NUD_Blocks.Value;

        AmountChange = (stacks * _material.MaxStackSize) + blocks;

        if(AmountChange == 0)
        {
            Log.Debug("{LogContext} Save ignored because amount change is zero. ItemId={ItemId}", LogContext, _material.ItemId);
            return;
        }

        Log.Debug("{LogContext} Collected amount adjustment confirmed. ProjectId={ProjectId}, ItemId={ItemId}, AmountChange={AmountChange}", LogContext, _material.ProjectId, _material.ItemId, AmountChange);

        DialogResult = DialogResult.OK;
        Close();
    }

    private void BTN_Cancel_Click(object sender, EventArgs e)
    {
        Log.Debug("{LogContext} Adjust collected amount dialogue cancelled. ItemId={ItemId}", LogContext, _material.ItemId);

        DialogResult = DialogResult.Cancel;
        Close();
    }
}
