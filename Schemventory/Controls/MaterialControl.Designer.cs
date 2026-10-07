namespace Schemventory.Controls;

partial class MaterialControl
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges9 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        PNL_Background = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
        IBN_MoreOptions = new Guna.UI2.WinForms.Guna2ImageButton();
        PBX_StateIcon = new PictureBox();
        LBL_TotalFormatted = new Label();
        LBL_BlocksNeeded = new Label();
        LBL_MaxStackSize = new Label();
        LBL_MaterialName = new Label();
        PBX_MaterialIcon = new PictureBox();
        TTP_Main = new ToolTip(components);
        CMS_MaterialOptions = new Guna.UI2.WinForms.Guna2ContextMenuStrip();
        TMI_SetState = new ToolStripMenuItem();
        CMS_SetState = new Guna.UI2.WinForms.Guna2ContextMenuStrip();
        TMI_SetStateCollected = new ToolStripMenuItem();
        TMI_SetStateMissing = new ToolStripMenuItem();
        TMI_SetStateIgnore = new ToolStripMenuItem();
        TMI_SetCollectedAmount = new ToolStripMenuItem();
        TMI_ReplaceMaterial = new ToolStripMenuItem();
        PNL_Background.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)PBX_StateIcon).BeginInit();
        ((System.ComponentModel.ISupportInitialize)PBX_MaterialIcon).BeginInit();
        CMS_MaterialOptions.SuspendLayout();
        CMS_SetState.SuspendLayout();
        SuspendLayout();
        // 
        // PNL_Background
        // 
        PNL_Background.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        PNL_Background.BackColor = Color.Transparent;
        PNL_Background.BorderColor = Color.FromArgb(103, 99, 99);
        PNL_Background.BorderRadius = 5;
        PNL_Background.BorderThickness = 2;
        PNL_Background.Controls.Add(IBN_MoreOptions);
        PNL_Background.Controls.Add(PBX_StateIcon);
        PNL_Background.Controls.Add(LBL_TotalFormatted);
        PNL_Background.Controls.Add(LBL_BlocksNeeded);
        PNL_Background.Controls.Add(LBL_MaxStackSize);
        PNL_Background.Controls.Add(LBL_MaterialName);
        PNL_Background.Controls.Add(PBX_MaterialIcon);
        PNL_Background.CustomBorderColor = Color.FromArgb(103, 99, 99);
        PNL_Background.CustomizableEdges = customizableEdges8;
        PNL_Background.FillColor = Color.FromArgb(58, 55, 55);
        PNL_Background.FillColor2 = Color.FromArgb(58, 55, 55);
        PNL_Background.FillColor3 = Color.FromArgb(58, 55, 55);
        PNL_Background.FillColor4 = Color.FromArgb(58, 55, 55);
        PNL_Background.Font = new Font("Leelawadee UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        PNL_Background.ForeColor = Color.FromArgb(235, 234, 234);
        PNL_Background.Location = new Point(3, 3);
        PNL_Background.Name = "PNL_Background";
        PNL_Background.ShadowDecoration.CustomizableEdges = customizableEdges9;
        PNL_Background.Size = new Size(306, 91);
        PNL_Background.TabIndex = 0;
        // 
        // IBN_MoreOptions
        // 
        IBN_MoreOptions.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        IBN_MoreOptions.CheckedState.ImageSize = new Size(25, 25);
        IBN_MoreOptions.HoverState.ImageSize = new Size(25, 25);
        IBN_MoreOptions.Image = Properties.Resources.MenuIcon;
        IBN_MoreOptions.ImageOffset = new Point(0, 0);
        IBN_MoreOptions.ImageRotate = 0F;
        IBN_MoreOptions.ImageSize = new Size(25, 25);
        IBN_MoreOptions.Location = new Point(277, 5);
        IBN_MoreOptions.Name = "IBN_MoreOptions";
        IBN_MoreOptions.PressedState.ImageSize = new Size(25, 25);
        IBN_MoreOptions.ShadowDecoration.CustomizableEdges = customizableEdges7;
        IBN_MoreOptions.Size = new Size(25, 25);
        IBN_MoreOptions.TabIndex = 2;
        TTP_Main.SetToolTip(IBN_MoreOptions, "Material options");
        IBN_MoreOptions.UseTransparentBackground = true;
        IBN_MoreOptions.Visible = false;
        IBN_MoreOptions.Click += IBN_MoreOptions_Click;
        // 
        // PBX_StateIcon
        // 
        PBX_StateIcon.Image = Properties.Resources.Checked;
        PBX_StateIcon.Location = new Point(9, 7);
        PBX_StateIcon.Name = "PBX_StateIcon";
        PBX_StateIcon.Size = new Size(35, 35);
        PBX_StateIcon.SizeMode = PictureBoxSizeMode.AutoSize;
        PBX_StateIcon.TabIndex = 1;
        PBX_StateIcon.TabStop = false;
        PBX_StateIcon.Visible = false;
        // 
        // LBL_TotalFormatted
        // 
        LBL_TotalFormatted.AutoSize = true;
        LBL_TotalFormatted.Font = new Font("Leelawadee UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
        LBL_TotalFormatted.ForeColor = Color.FromArgb(217, 74, 74);
        LBL_TotalFormatted.Location = new Point(76, 41);
        LBL_TotalFormatted.Name = "LBL_TotalFormatted";
        LBL_TotalFormatted.Size = new Size(190, 17);
        LBL_TotalFormatted.TabIndex = 0;
        LBL_TotalFormatted.Text = "Missing: XX Stacks && XX Blocks";
        // 
        // LBL_BlocksNeeded
        // 
        LBL_BlocksNeeded.AutoSize = true;
        LBL_BlocksNeeded.Font = new Font("Leelawadee UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
        LBL_BlocksNeeded.ForeColor = Color.FromArgb(151, 148, 148);
        LBL_BlocksNeeded.Location = new Point(76, 26);
        LBL_BlocksNeeded.Name = "LBL_BlocksNeeded";
        LBL_BlocksNeeded.Size = new Size(68, 15);
        LBL_BlocksNeeded.TabIndex = 0;
        LBL_BlocksNeeded.Text = "Total: XXXX";
        // 
        // LBL_MaxStackSize
        // 
        LBL_MaxStackSize.AutoSize = true;
        LBL_MaxStackSize.Font = new Font("Leelawadee UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
        LBL_MaxStackSize.ForeColor = Color.FromArgb(151, 148, 148);
        LBL_MaxStackSize.Location = new Point(3, 72);
        LBL_MaxStackSize.Name = "LBL_MaxStackSize";
        LBL_MaxStackSize.Size = new Size(77, 15);
        LBL_MaxStackSize.TabIndex = 0;
        LBL_MaxStackSize.Text = "Stack size: XX";
        // 
        // LBL_MaterialName
        // 
        LBL_MaterialName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        LBL_MaterialName.AutoEllipsis = true;
        LBL_MaterialName.Font = new Font("Leelawadee UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
        LBL_MaterialName.ForeColor = Color.FromArgb(194, 165, 142);
        LBL_MaterialName.Location = new Point(76, 5);
        LBL_MaterialName.Name = "LBL_MaterialName";
        LBL_MaterialName.Size = new Size(195, 21);
        LBL_MaterialName.TabIndex = 0;
        LBL_MaterialName.Text = "Material Name";
        LBL_MaterialName.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // PBX_MaterialIcon
        // 
        PBX_MaterialIcon.Location = new Point(9, 7);
        PBX_MaterialIcon.Name = "PBX_MaterialIcon";
        PBX_MaterialIcon.Size = new Size(64, 64);
        PBX_MaterialIcon.TabIndex = 0;
        PBX_MaterialIcon.TabStop = false;
        // 
        // TTP_Main
        // 
        TTP_Main.AutomaticDelay = 200;
        TTP_Main.AutoPopDelay = 99999;
        TTP_Main.BackColor = Color.FromArgb(38, 34, 34);
        TTP_Main.ForeColor = Color.FromArgb(235, 234, 234);
        TTP_Main.InitialDelay = 1300;
        TTP_Main.ReshowDelay = 0;
        TTP_Main.ToolTipIcon = ToolTipIcon.Info;
        TTP_Main.ToolTipTitle = "Information";
        // 
        // CMS_MaterialOptions
        // 
        CMS_MaterialOptions.BackColor = Color.FromArgb(58, 55, 55);
        CMS_MaterialOptions.Font = new Font("Leelawadee UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
        CMS_MaterialOptions.Items.AddRange(new ToolStripItem[] { TMI_SetState, TMI_SetCollectedAmount, TMI_ReplaceMaterial });
        CMS_MaterialOptions.Name = "CMS_MaterialOptions";
        CMS_MaterialOptions.RenderStyle.ArrowColor = Color.FromArgb(194, 165, 142);
        CMS_MaterialOptions.RenderStyle.BorderColor = Color.FromArgb(103, 99, 99);
        CMS_MaterialOptions.RenderStyle.ColorTable = null;
        CMS_MaterialOptions.RenderStyle.RoundedEdges = true;
        CMS_MaterialOptions.RenderStyle.SelectionArrowColor = Color.FromArgb(194, 165, 142);
        CMS_MaterialOptions.RenderStyle.SelectionBackColor = Color.FromArgb(162, 123, 90);
        CMS_MaterialOptions.RenderStyle.SelectionForeColor = Color.White;
        CMS_MaterialOptions.RenderStyle.SeparatorColor = Color.FromArgb(151, 148, 148);
        CMS_MaterialOptions.RenderStyle.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
        CMS_MaterialOptions.Size = new Size(217, 92);
        // 
        // TMI_SetState
        // 
        TMI_SetState.DropDown = CMS_SetState;
        TMI_SetState.ForeColor = Color.FromArgb(235, 234, 234);
        TMI_SetState.Name = "TMI_SetState";
        TMI_SetState.Size = new Size(216, 22);
        TMI_SetState.Text = "Status";
        // 
        // CMS_SetState
        // 
        CMS_SetState.BackColor = Color.FromArgb(58, 55, 55);
        CMS_SetState.Font = new Font("Leelawadee UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
        CMS_SetState.Items.AddRange(new ToolStripItem[] { TMI_SetStateCollected, TMI_SetStateMissing, TMI_SetStateIgnore });
        CMS_SetState.Name = "CMS_MaterialOptions";
        CMS_SetState.OwnerItem = TMI_SetState;
        CMS_SetState.RenderStyle.ArrowColor = Color.FromArgb(194, 165, 142);
        CMS_SetState.RenderStyle.BorderColor = Color.FromArgb(103, 99, 99);
        CMS_SetState.RenderStyle.ColorTable = null;
        CMS_SetState.RenderStyle.RoundedEdges = true;
        CMS_SetState.RenderStyle.SelectionArrowColor = Color.FromArgb(194, 165, 142);
        CMS_SetState.RenderStyle.SelectionBackColor = Color.FromArgb(162, 123, 90);
        CMS_SetState.RenderStyle.SelectionForeColor = Color.White;
        CMS_SetState.RenderStyle.SeparatorColor = Color.FromArgb(151, 148, 148);
        CMS_SetState.RenderStyle.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
        CMS_SetState.Size = new Size(131, 70);
        // 
        // TMI_SetStateCollected
        // 
        TMI_SetStateCollected.ForeColor = Color.FromArgb(235, 234, 234);
        TMI_SetStateCollected.Name = "TMI_SetStateCollected";
        TMI_SetStateCollected.Size = new Size(130, 22);
        TMI_SetStateCollected.Text = "Collected";
        TMI_SetStateCollected.Click += TMI_SetStateCollected_Click;
        // 
        // TMI_SetStateMissing
        // 
        TMI_SetStateMissing.ForeColor = Color.FromArgb(235, 234, 234);
        TMI_SetStateMissing.Name = "TMI_SetStateMissing";
        TMI_SetStateMissing.Size = new Size(130, 22);
        TMI_SetStateMissing.Text = "Missing";
        TMI_SetStateMissing.Click += TMI_SetStateMissing_Click;
        // 
        // TMI_SetStateIgnore
        // 
        TMI_SetStateIgnore.ForeColor = Color.FromArgb(235, 234, 234);
        TMI_SetStateIgnore.Name = "TMI_SetStateIgnore";
        TMI_SetStateIgnore.Size = new Size(130, 22);
        TMI_SetStateIgnore.Text = "Ignore";
        TMI_SetStateIgnore.Click += TMI_SetStateIgnored_Click;
        // 
        // TMI_SetCollectedAmount
        // 
        TMI_SetCollectedAmount.ForeColor = Color.FromArgb(235, 234, 234);
        TMI_SetCollectedAmount.Name = "TMI_SetCollectedAmount";
        TMI_SetCollectedAmount.Size = new Size(216, 22);
        TMI_SetCollectedAmount.Text = "Adjust collected amount";
        TMI_SetCollectedAmount.Click += TMI_SetCollectedAmount_Click;
        // 
        // TMI_ReplaceMaterial
        // 
        TMI_ReplaceMaterial.ForeColor = Color.FromArgb(235, 234, 234);
        TMI_ReplaceMaterial.Name = "TMI_ReplaceMaterial";
        TMI_ReplaceMaterial.Size = new Size(216, 22);
        TMI_ReplaceMaterial.Text = "Replace material";
        TMI_ReplaceMaterial.Click += TMI_ReplaceMaterial_Click;
        // 
        // MaterialControl
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.Transparent;
        Controls.Add(PNL_Background);
        Cursor = Cursors.Hand;
        Name = "MaterialControl";
        Size = new Size(312, 97);
        Load += MaterialControl_Load;
        PNL_Background.ResumeLayout(false);
        PNL_Background.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)PBX_StateIcon).EndInit();
        ((System.ComponentModel.ISupportInitialize)PBX_MaterialIcon).EndInit();
        CMS_MaterialOptions.ResumeLayout(false);
        CMS_SetState.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion

    private Guna.UI2.WinForms.Guna2CustomGradientPanel PNL_Background;
    private PictureBox PBX_MaterialIcon;
    private Label LBL_MaterialName;
    private Label LBL_MaxStackSize;
    private Label LBL_BlocksNeeded;
    private Label LBL_TotalFormatted;
    private PictureBox PBX_StateIcon;
    private ToolTip TTP_Main;
    private Guna.UI2.WinForms.Guna2ContextMenuStrip CMS_MaterialOptions;
    private ToolStripMenuItem TMI_SetState;
    private Guna.UI2.WinForms.Guna2ImageButton IBN_MoreOptions;
    private Guna.UI2.WinForms.Guna2ContextMenuStrip CMS_SetState;
    private ToolStripMenuItem TMI_SetStateMissing;
    private ToolStripMenuItem TMI_SetStateCollected;
    private ToolStripMenuItem TMI_SetStateIgnore;
    private ToolStripMenuItem TMI_SetCollectedAmount;
    private ToolStripMenuItem TMI_ReplaceMaterial;
}
