namespace Schemventory.Forms;

partial class FRM_AdjustCollectedAmount
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if(disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FRM_AdjustCollectedAmount));
        NUD_Stacks = new Guna.UI2.WinForms.Guna2NumericUpDown();
        NUD_Blocks = new Guna.UI2.WinForms.Guna2NumericUpDown();
        LBL_Stacks = new Label();
        LBL_Blocks = new Label();
        LBL_Heading = new Label();
        BTN_Save = new Guna.UI2.WinForms.Guna2Button();
        BTN_Cancel = new Guna.UI2.WinForms.Guna2Button();
        LBL_CollectedStats = new Label();
        ((System.ComponentModel.ISupportInitialize)NUD_Stacks).BeginInit();
        ((System.ComponentModel.ISupportInitialize)NUD_Blocks).BeginInit();
        SuspendLayout();
        // 
        // NUD_Stacks
        // 
        NUD_Stacks.BackColor = Color.Transparent;
        NUD_Stacks.BorderColor = Color.FromArgb(103, 99, 99);
        NUD_Stacks.BorderRadius = 5;
        NUD_Stacks.BorderThickness = 2;
        NUD_Stacks.CustomizableEdges = customizableEdges1;
        NUD_Stacks.FillColor = Color.FromArgb(58, 55, 55);
        NUD_Stacks.FocusedState.BorderColor = Color.FromArgb(162, 123, 90);
        NUD_Stacks.Font = new Font("Leelawadee UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        NUD_Stacks.ForeColor = Color.FromArgb(235, 234, 234);
        NUD_Stacks.Location = new Point(55, 110);
        NUD_Stacks.Name = "NUD_Stacks";
        NUD_Stacks.ShadowDecoration.CustomizableEdges = customizableEdges2;
        NUD_Stacks.Size = new Size(73, 36);
        NUD_Stacks.TabIndex = 1;
        NUD_Stacks.UpDownButtonFillColor = Color.FromArgb(162, 123, 90);
        NUD_Stacks.UpDownButtonForeColor = Color.FromArgb(235, 234, 234);
        NUD_Stacks.ValueChanged += NUD_Stacks_ValueChanged;
        // 
        // NUD_Blocks
        // 
        NUD_Blocks.BackColor = Color.Transparent;
        NUD_Blocks.BorderColor = Color.FromArgb(103, 99, 99);
        NUD_Blocks.BorderRadius = 5;
        NUD_Blocks.BorderThickness = 2;
        NUD_Blocks.CustomizableEdges = customizableEdges3;
        NUD_Blocks.FillColor = Color.FromArgb(58, 55, 55);
        NUD_Blocks.FocusedState.BorderColor = Color.FromArgb(162, 123, 90);
        NUD_Blocks.Font = new Font("Leelawadee UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        NUD_Blocks.ForeColor = Color.FromArgb(235, 234, 234);
        NUD_Blocks.Location = new Point(227, 110);
        NUD_Blocks.Name = "NUD_Blocks";
        NUD_Blocks.ShadowDecoration.CustomizableEdges = customizableEdges4;
        NUD_Blocks.Size = new Size(73, 36);
        NUD_Blocks.TabIndex = 1;
        NUD_Blocks.UpDownButtonFillColor = Color.FromArgb(162, 123, 90);
        NUD_Blocks.UpDownButtonForeColor = Color.FromArgb(235, 234, 234);
        NUD_Blocks.ValueChanged += NUD_Blocks_ValueChanged;
        // 
        // LBL_Stacks
        // 
        LBL_Stacks.AutoSize = true;
        LBL_Stacks.Font = new Font("Leelawadee UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        LBL_Stacks.ForeColor = Color.FromArgb(235, 234, 234);
        LBL_Stacks.Location = new Point(134, 118);
        LBL_Stacks.Name = "LBL_Stacks";
        LBL_Stacks.Size = new Size(50, 20);
        LBL_Stacks.TabIndex = 2;
        LBL_Stacks.Text = "Stacks";
        // 
        // LBL_Blocks
        // 
        LBL_Blocks.AutoSize = true;
        LBL_Blocks.Font = new Font("Leelawadee UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        LBL_Blocks.ForeColor = Color.FromArgb(235, 234, 234);
        LBL_Blocks.Location = new Point(306, 118);
        LBL_Blocks.Name = "LBL_Blocks";
        LBL_Blocks.Size = new Size(51, 20);
        LBL_Blocks.TabIndex = 2;
        LBL_Blocks.Text = "Blocks";
        // 
        // LBL_Heading
        // 
        LBL_Heading.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        LBL_Heading.Font = new Font("Leelawadee UI", 20.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        LBL_Heading.ForeColor = Color.FromArgb(162, 123, 90);
        LBL_Heading.Location = new Point(12, 9);
        LBL_Heading.Name = "LBL_Heading";
        LBL_Heading.Size = new Size(389, 37);
        LBL_Heading.TabIndex = 3;
        LBL_Heading.Text = "Adjust collected amount";
        LBL_Heading.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // BTN_Save
        // 
        BTN_Save.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        BTN_Save.Animated = true;
        BTN_Save.BackColor = Color.Transparent;
        BTN_Save.BorderRadius = 5;
        BTN_Save.Cursor = Cursors.Hand;
        BTN_Save.CustomizableEdges = customizableEdges5;
        BTN_Save.DialogResult = DialogResult.OK;
        BTN_Save.DisabledState.BorderColor = Color.FromArgb(80, 76, 76);
        BTN_Save.DisabledState.CustomBorderColor = Color.FromArgb(80, 76, 76);
        BTN_Save.DisabledState.FillColor = Color.FromArgb(80, 76, 76);
        BTN_Save.DisabledState.ForeColor = Color.FromArgb(151, 148, 148);
        BTN_Save.FillColor = Color.FromArgb(162, 123, 90);
        BTN_Save.Font = new Font("Leelawadee UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        BTN_Save.ForeColor = Color.White;
        BTN_Save.Location = new Point(306, 212);
        BTN_Save.Name = "BTN_Save";
        BTN_Save.ShadowDecoration.CustomizableEdges = customizableEdges6;
        BTN_Save.Size = new Size(95, 32);
        BTN_Save.TabIndex = 4;
        BTN_Save.Text = "Save";
        BTN_Save.UseTransparentBackground = true;
        BTN_Save.Click += BTN_Save_Click;
        // 
        // BTN_Cancel
        // 
        BTN_Cancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        BTN_Cancel.Animated = true;
        BTN_Cancel.BackColor = Color.Transparent;
        BTN_Cancel.BorderColor = Color.FromArgb(80, 76, 76);
        BTN_Cancel.BorderRadius = 5;
        BTN_Cancel.BorderThickness = 2;
        BTN_Cancel.Cursor = Cursors.Hand;
        BTN_Cancel.CustomizableEdges = customizableEdges7;
        BTN_Cancel.DisabledState.BorderColor = Color.FromArgb(80, 76, 76);
        BTN_Cancel.DisabledState.CustomBorderColor = Color.FromArgb(80, 76, 76);
        BTN_Cancel.DisabledState.FillColor = Color.FromArgb(80, 76, 76);
        BTN_Cancel.DisabledState.ForeColor = Color.FromArgb(151, 148, 148);
        BTN_Cancel.FillColor = Color.FromArgb(80, 76, 76);
        BTN_Cancel.Font = new Font("Leelawadee UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        BTN_Cancel.ForeColor = Color.White;
        BTN_Cancel.HoverState.BorderColor = Color.FromArgb(103, 99, 99);
        BTN_Cancel.Location = new Point(12, 212);
        BTN_Cancel.Name = "BTN_Cancel";
        BTN_Cancel.ShadowDecoration.CustomizableEdges = customizableEdges8;
        BTN_Cancel.Size = new Size(119, 32);
        BTN_Cancel.TabIndex = 5;
        BTN_Cancel.Text = "Cancel";
        BTN_Cancel.UseTransparentBackground = true;
        BTN_Cancel.Click += BTN_Cancel_Click;
        // 
        // LBL_CollectedStats
        // 
        LBL_CollectedStats.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        LBL_CollectedStats.AutoEllipsis = true;
        LBL_CollectedStats.Font = new Font("Leelawadee UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
        LBL_CollectedStats.ForeColor = Color.FromArgb(194, 165, 142);
        LBL_CollectedStats.Location = new Point(12, 46);
        LBL_CollectedStats.Name = "LBL_CollectedStats";
        LBL_CollectedStats.Size = new Size(389, 21);
        LBL_CollectedStats.TabIndex = 6;
        LBL_CollectedStats.Text = "SubHeading";
        LBL_CollectedStats.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // FRM_AdjustCollectedAmount
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(38, 34, 34);
        ClientSize = new Size(413, 256);
        Controls.Add(LBL_CollectedStats);
        Controls.Add(BTN_Cancel);
        Controls.Add(BTN_Save);
        Controls.Add(LBL_Heading);
        Controls.Add(LBL_Blocks);
        Controls.Add(LBL_Stacks);
        Controls.Add(NUD_Blocks);
        Controls.Add(NUD_Stacks);
        Font = new Font("Leelawadee UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        ForeColor = Color.FromArgb(235, 234, 234);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        Icon = (Icon)resources.GetObject("$this.Icon");
        Margin = new Padding(3, 4, 3, 4);
        MaximizeBox = false;
        Name = "FRM_AdjustCollectedAmount";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "FRM_SetCollectedAmount";
        ((System.ComponentModel.ISupportInitialize)NUD_Stacks).EndInit();
        ((System.ComponentModel.ISupportInitialize)NUD_Blocks).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Guna.UI2.WinForms.Guna2NumericUpDown NUD_Stacks;
    private Guna.UI2.WinForms.Guna2NumericUpDown NUD_Blocks;
    private Label LBL_Stacks;
    private Label LBL_Blocks;
    private Label LBL_Heading;
    private Guna.UI2.WinForms.Guna2Button BTN_Save;
    private Guna.UI2.WinForms.Guna2Button BTN_Cancel;
    private Label LBL_CollectedStats;
}