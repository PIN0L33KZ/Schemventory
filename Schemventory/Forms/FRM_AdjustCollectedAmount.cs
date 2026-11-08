using Guna.UI2.WinForms;
using Schemventory.App;
using Serilog;

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

        Log.Debug(
            "{LogContext} Adjust collected amount dialog opened. ProjectId={ProjectId}, ItemId={ItemId}, CollectedAmount={CollectedAmount}, RequiredAmount={RequiredAmount}",
            LogContext,
            _material.ProjectId,
            _material.ItemId,
            _material.CollectedAmount,
            _material.RequiredAmount);
    }

    private void InitializeValues()
    {
        _updatingValues = true;

        try
        {
            NUD_Stacks.Value = 0;
            NUD_Blocks.Value = 0;
            _direction = AmountDirection.None;

            UpdateLimits();
            UpdateHeading();
            UpdateCollectedStats();
        }
        finally
        {
            _updatingValues = false;
        }
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

        _updatingValues = true;

        try
        {
            if(_direction == AmountDirection.None)
            {
                if(changedControl.Value > 0)
                    _direction = AmountDirection.Add;
                else if(changedControl.Value < 0)
                    _direction = AmountDirection.Remove;
            }

            if(NUD_Stacks.Value == 0 && NUD_Blocks.Value == 0)
                _direction = AmountDirection.None;

            UpdateLimits();
            UpdateHeading();
        }
        finally
        {
            _updatingValues = false;
        }
    }

    private void UpdateLimits()
    {
        var stackSize = GetStackSize();
        var maxAddAmount = Math.Max(0, _material.RequiredAmount - _material.CollectedAmount);
        var maxRemoveAmount = Math.Max(0, _material.CollectedAmount);

        switch(_direction)
        {
            case AmountDirection.Add:
                UpdateAddLimits(stackSize, maxAddAmount);
                break;

            case AmountDirection.Remove:
                UpdateRemoveLimits(stackSize, maxRemoveAmount);
                break;

            default:
                UpdateNeutralLimits(stackSize, maxAddAmount, maxRemoveAmount);
                break;
        }

        BTN_Save.Enabled = GetAmountChange() != 0;
    }

    private void UpdateAddLimits(long stackSize, long maxAddAmount)
    {
        var maxStacks = maxAddAmount / stackSize;

        SetRange(NUD_Stacks, 0, maxStacks);

        var stackAmount = (long)NUD_Stacks.Value * stackSize;
        var remainingAmount = Math.Max(0, maxAddAmount - stackAmount);
        var maxBlocks = Math.Min(stackSize - 1, remainingAmount);

        SetRange(NUD_Blocks, 0, maxBlocks);
    }

    private void UpdateRemoveLimits(long stackSize, long maxRemoveAmount)
    {
        var maxStacks = maxRemoveAmount / stackSize;

        SetRange(NUD_Stacks, -maxStacks, 0);

        var stackAmount = Math.Abs((long)NUD_Stacks.Value) * stackSize;
        var remainingAmount = Math.Max(0, maxRemoveAmount - stackAmount);
        var maxBlocks = Math.Min(stackSize - 1, remainingAmount);

        SetRange(NUD_Blocks, -maxBlocks, 0);
    }

    private void UpdateNeutralLimits(long stackSize, long maxAddAmount, long maxRemoveAmount)
    {
        var maxAddStacks = maxAddAmount / stackSize;
        var maxRemoveStacks = maxRemoveAmount / stackSize;
        var maxAddBlocks = Math.Min(stackSize - 1, maxAddAmount);
        var maxRemoveBlocks = Math.Min(stackSize - 1, maxRemoveAmount);

        SetRange(NUD_Stacks, -maxRemoveStacks, maxAddStacks);
        SetRange(NUD_Blocks, -maxRemoveBlocks, maxAddBlocks);
    }

    private static void SetRange(Guna2NumericUpDown numericUpDown, decimal minimum, decimal maximum)
    {
        if(minimum > maximum)
            throw new ArgumentOutOfRangeException(nameof(minimum));

        if(minimum < numericUpDown.Minimum)
            numericUpDown.Minimum = minimum;

        if(maximum > numericUpDown.Maximum)
            numericUpDown.Maximum = maximum;

        var value = Math.Clamp(numericUpDown.Value, minimum, maximum);

        numericUpDown.Value = value;
        numericUpDown.Minimum = minimum;
        numericUpDown.Maximum = maximum;
    }

    private long GetAmountChange()
    {
        var stacks = (long)NUD_Stacks.Value;
        var blocks = (long)NUD_Blocks.Value;

        return checked((stacks * GetStackSize()) + blocks);
    }

    private long GetStackSize()
    {
        return Math.Max(1, _material.MaxStackSize);
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
        var stackSize = GetStackSize();
        var collectedAmount = _material.CollectedAmount;
        var stacks = collectedAmount / stackSize;
        var blocks = collectedAmount % stackSize;

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
        long amountChange;

        try
        {
            amountChange = GetAmountChange();
        }
        catch(OverflowException exception)
        {
            Log.Warning(
                exception,
                "{LogContext} Collected amount adjustment overflowed. ProjectId={ProjectId}, ItemId={ItemId}",
                LogContext,
                _material.ProjectId,
                _material.ItemId);

            _ = MessageBox.Show(
                "The selected amount is too large.",
                WindowName,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            return;
        }

        if(amountChange == 0)
        {
            Log.Debug("{LogContext} Save ignored because amount change is zero. ItemId={ItemId}", LogContext, _material.ItemId);
            return;
        }

        var maxAddAmount = Math.Max(0, _material.RequiredAmount - _material.CollectedAmount);
        var maxRemoveAmount = Math.Max(0, _material.CollectedAmount);

        if(amountChange > maxAddAmount || amountChange < -maxRemoveAmount)
        {
            Log.Warning(
                "{LogContext} Collected amount adjustment is outside the valid range. ProjectId={ProjectId}, ItemId={ItemId}, AmountChange={AmountChange}, MaxAddAmount={MaxAddAmount}, MaxRemoveAmount={MaxRemoveAmount}",
                LogContext,
                _material.ProjectId,
                _material.ItemId,
                amountChange,
                maxAddAmount,
                maxRemoveAmount);

            _ = MessageBox.Show(
                "The selected amount is outside the valid range.",
                WindowName,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            return;
        }

        AmountChange = amountChange;

        Log.Debug(
            "{LogContext} Collected amount adjustment confirmed. ProjectId={ProjectId}, ItemId={ItemId}, AmountChange={AmountChange}",
            LogContext,
            _material.ProjectId,
            _material.ItemId,
            AmountChange);

        DialogResult = DialogResult.OK;
        Close();
    }

    private void BTN_Cancel_Click(object sender, EventArgs e)
    {
        Log.Debug("{LogContext} Adjust collected amount dialog cancelled. ItemId={ItemId}", LogContext, _material.ItemId);

        DialogResult = DialogResult.Cancel;
        Close();
    }

    private void FRM_AdjustCollectedAmount_KeyDown(object sender, KeyEventArgs e)
    {
        if(e.KeyCode == Keys.Escape)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}