namespace Schemventory.Forms;

partial class FRM_MaterialList
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
        components = new System.ComponentModel.Container();
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FRM_MaterialList));
        PNL_ProjectInfo = new Guna.UI2.WinForms.Guna2Panel();
        LBL_DifferentMaterialsCount = new Label();
        LBL_TotalBlocks = new Label();
        LBL_ProjectName = new Label();
        PNL_MaterialList = new FlowLayoutPanel();
        VSB_Main = new Guna.UI2.WinForms.Guna2VScrollBar();
        CHB_ShowMissing = new Guna.UI2.WinForms.Guna2CheckBox();
        CBX_ShowReplaced = new Guna.UI2.WinForms.Guna2CheckBox();
        CBX_ShowCollected = new Guna.UI2.WinForms.Guna2CheckBox();
        CBX_ShowIgnored = new Guna.UI2.WinForms.Guna2CheckBox();
        IBN_About = new Guna.UI2.WinForms.Guna2ImageButton();
        TTP_Main = new ToolTip(components);
        PNL_ProjectInfo.SuspendLayout();
        SuspendLayout();
        // 
        // PNL_ProjectInfo
        // 
        PNL_ProjectInfo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        PNL_ProjectInfo.BackColor = Color.Transparent;
        PNL_ProjectInfo.BorderColor = Color.FromArgb(103, 99, 99);
        PNL_ProjectInfo.BorderRadius = 5;
        PNL_ProjectInfo.BorderThickness = 2;
        PNL_ProjectInfo.Controls.Add(LBL_DifferentMaterialsCount);
        PNL_ProjectInfo.Controls.Add(LBL_TotalBlocks);
        PNL_ProjectInfo.Controls.Add(LBL_ProjectName);
        PNL_ProjectInfo.CustomizableEdges = customizableEdges1;
        PNL_ProjectInfo.FillColor = Color.FromArgb(58, 55, 55);
        PNL_ProjectInfo.Location = new Point(12, 12);
        PNL_ProjectInfo.Name = "PNL_ProjectInfo";
        PNL_ProjectInfo.ShadowDecoration.CustomizableEdges = customizableEdges2;
        PNL_ProjectInfo.Size = new Size(975, 55);
        PNL_ProjectInfo.TabIndex = 0;
        PNL_ProjectInfo.UseTransparentBackground = true;
        // 
        // LBL_DifferentMaterialsCount
        // 
        LBL_DifferentMaterialsCount.AutoEllipsis = true;
        LBL_DifferentMaterialsCount.AutoSize = true;
        LBL_DifferentMaterialsCount.Font = new Font("Leelawadee UI", 11.25F, FontStyle.Bold);
        LBL_DifferentMaterialsCount.ForeColor = Color.FromArgb(235, 234, 234);
        LBL_DifferentMaterialsCount.Location = new Point(13, 27);
        LBL_DifferentMaterialsCount.Name = "LBL_DifferentMaterialsCount";
        LBL_DifferentMaterialsCount.Size = new Size(309, 20);
        LBL_DifferentMaterialsCount.TabIndex = 0;
        LBL_DifferentMaterialsCount.Text = "Different material count: DifferentMaterial";
        TTP_Main.SetToolTip(LBL_DifferentMaterialsCount, "Amount of different materials used in this project");
        // 
        // LBL_TotalBlocks
        // 
        LBL_TotalBlocks.AutoEllipsis = true;
        LBL_TotalBlocks.AutoSize = true;
        LBL_TotalBlocks.Font = new Font("Leelawadee UI", 11.25F, FontStyle.Bold);
        LBL_TotalBlocks.ForeColor = Color.FromArgb(235, 234, 234);
        LBL_TotalBlocks.Location = new Point(13, 7);
        LBL_TotalBlocks.Name = "LBL_TotalBlocks";
        LBL_TotalBlocks.Size = new Size(218, 20);
        LBL_TotalBlocks.TabIndex = 0;
        LBL_TotalBlocks.Text = "Total block count: BlockCount";
        TTP_Main.SetToolTip(LBL_TotalBlocks, "Amout of blocks used in this project");
        // 
        // LBL_ProjectName
        // 
        LBL_ProjectName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        LBL_ProjectName.AutoEllipsis = true;
        LBL_ProjectName.Font = new Font("Leelawadee UI", 20.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        LBL_ProjectName.ForeColor = Color.FromArgb(162, 123, 90);
        LBL_ProjectName.Location = new Point(328, 7);
        LBL_ProjectName.Name = "LBL_ProjectName";
        LBL_ProjectName.Size = new Size(635, 37);
        LBL_ProjectName.TabIndex = 0;
        LBL_ProjectName.Text = "Project name";
        LBL_ProjectName.TextAlign = ContentAlignment.MiddleRight;
        // 
        // PNL_MaterialList
        // 
        PNL_MaterialList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        PNL_MaterialList.AutoScroll = true;
        PNL_MaterialList.Location = new Point(12, 73);
        PNL_MaterialList.Name = "PNL_MaterialList";
        PNL_MaterialList.Size = new Size(975, 416);
        PNL_MaterialList.TabIndex = 0;
        // 
        // VSB_Main
        // 
        VSB_Main.BackColor = Color.Transparent;
        VSB_Main.BindingContainer = PNL_MaterialList;
        VSB_Main.BorderRadius = 5;
        VSB_Main.FillColor = Color.FromArgb(80, 76, 76);
        VSB_Main.InUpdate = false;
        VSB_Main.LargeChange = 10;
        VSB_Main.Location = new Point(969, 73);
        VSB_Main.Name = "VSB_Main";
        VSB_Main.ScrollbarSize = 18;
        VSB_Main.Size = new Size(18, 416);
        VSB_Main.TabIndex = 0;
        VSB_Main.TabStop = false;
        VSB_Main.ThumbColor = Color.FromArgb(103, 99, 99);
        VSB_Main.ThumbSize = 5F;
        VSB_Main.ThumbStyle = Guna.UI2.WinForms.Enums.ThumbStyle.Inset;
        // 
        // CHB_ShowMissing
        // 
        CHB_ShowMissing.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        CHB_ShowMissing.Animated = true;
        CHB_ShowMissing.AutoSize = true;
        CHB_ShowMissing.BackColor = Color.Transparent;
        CHB_ShowMissing.Checked = true;
        CHB_ShowMissing.CheckedState.BorderRadius = 2;
        CHB_ShowMissing.CheckedState.BorderThickness = 0;
        CHB_ShowMissing.CheckedState.FillColor = Color.FromArgb(162, 123, 90);
        CHB_ShowMissing.CheckMarkColor = Color.FromArgb(235, 234, 234);
        CHB_ShowMissing.CheckState = CheckState.Checked;
        CHB_ShowMissing.Cursor = Cursors.Hand;
        CHB_ShowMissing.Font = new Font("Leelawadee UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        CHB_ShowMissing.ForeColor = Color.FromArgb(235, 234, 234);
        CHB_ShowMissing.Location = new Point(12, 495);
        CHB_ShowMissing.Name = "CHB_ShowMissing";
        CHB_ShowMissing.Size = new Size(143, 24);
        CHB_ShowMissing.TabIndex = 1;
        CHB_ShowMissing.Text = "Missing materials";
        TTP_Main.SetToolTip(CHB_ShowMissing, "Show/Hide missing materials");
        CHB_ShowMissing.UncheckedState.BorderColor = Color.FromArgb(103, 99, 99);
        CHB_ShowMissing.UncheckedState.BorderRadius = 2;
        CHB_ShowMissing.UncheckedState.BorderThickness = 2;
        CHB_ShowMissing.UncheckedState.FillColor = Color.FromArgb(58, 55, 55);
        CHB_ShowMissing.UseVisualStyleBackColor = false;
        CHB_ShowMissing.CheckedChanged += CHB_ShowMissing_CheckedChanged;
        // 
        // CBX_ShowReplaced
        // 
        CBX_ShowReplaced.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        CBX_ShowReplaced.Animated = true;
        CBX_ShowReplaced.AutoSize = true;
        CBX_ShowReplaced.BackColor = Color.Transparent;
        CBX_ShowReplaced.Checked = true;
        CBX_ShowReplaced.CheckedState.BorderRadius = 2;
        CBX_ShowReplaced.CheckedState.BorderThickness = 0;
        CBX_ShowReplaced.CheckedState.FillColor = Color.FromArgb(162, 123, 90);
        CBX_ShowReplaced.CheckMarkColor = Color.FromArgb(235, 234, 234);
        CBX_ShowReplaced.CheckState = CheckState.Checked;
        CBX_ShowReplaced.Cursor = Cursors.Hand;
        CBX_ShowReplaced.Font = new Font("Leelawadee UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        CBX_ShowReplaced.ForeColor = Color.FromArgb(235, 234, 234);
        CBX_ShowReplaced.Location = new Point(161, 495);
        CBX_ShowReplaced.Name = "CBX_ShowReplaced";
        CBX_ShowReplaced.Size = new Size(155, 24);
        CBX_ShowReplaced.TabIndex = 2;
        CBX_ShowReplaced.Text = "Replaced materials";
        TTP_Main.SetToolTip(CBX_ShowReplaced, "Show/Hide replaced materials");
        CBX_ShowReplaced.UncheckedState.BorderColor = Color.FromArgb(103, 99, 99);
        CBX_ShowReplaced.UncheckedState.BorderRadius = 2;
        CBX_ShowReplaced.UncheckedState.BorderThickness = 2;
        CBX_ShowReplaced.UncheckedState.FillColor = Color.FromArgb(58, 55, 55);
        CBX_ShowReplaced.UseVisualStyleBackColor = false;
        CBX_ShowReplaced.CheckedChanged += CBX_ShowReplaced_CheckedChanged;
        // 
        // CBX_ShowCollected
        // 
        CBX_ShowCollected.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        CBX_ShowCollected.Animated = true;
        CBX_ShowCollected.AutoSize = true;
        CBX_ShowCollected.BackColor = Color.Transparent;
        CBX_ShowCollected.CheckedState.BorderRadius = 2;
        CBX_ShowCollected.CheckedState.BorderThickness = 0;
        CBX_ShowCollected.CheckedState.FillColor = Color.FromArgb(162, 123, 90);
        CBX_ShowCollected.CheckMarkColor = Color.FromArgb(235, 234, 234);
        CBX_ShowCollected.Cursor = Cursors.Hand;
        CBX_ShowCollected.Font = new Font("Leelawadee UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        CBX_ShowCollected.ForeColor = Color.FromArgb(235, 234, 234);
        CBX_ShowCollected.Location = new Point(322, 495);
        CBX_ShowCollected.Name = "CBX_ShowCollected";
        CBX_ShowCollected.Size = new Size(156, 24);
        CBX_ShowCollected.TabIndex = 3;
        CBX_ShowCollected.Text = "Collected materials";
        TTP_Main.SetToolTip(CBX_ShowCollected, "Show/Hide collected materials");
        CBX_ShowCollected.UncheckedState.BorderColor = Color.FromArgb(103, 99, 99);
        CBX_ShowCollected.UncheckedState.BorderRadius = 2;
        CBX_ShowCollected.UncheckedState.BorderThickness = 2;
        CBX_ShowCollected.UncheckedState.FillColor = Color.FromArgb(58, 55, 55);
        CBX_ShowCollected.UseVisualStyleBackColor = false;
        CBX_ShowCollected.CheckedChanged += CBX_ShowCollected_CheckedChanged;
        // 
        // CBX_ShowIgnored
        // 
        CBX_ShowIgnored.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        CBX_ShowIgnored.Animated = true;
        CBX_ShowIgnored.AutoSize = true;
        CBX_ShowIgnored.BackColor = Color.Transparent;
        CBX_ShowIgnored.CheckedState.BorderRadius = 2;
        CBX_ShowIgnored.CheckedState.BorderThickness = 0;
        CBX_ShowIgnored.CheckedState.FillColor = Color.FromArgb(162, 123, 90);
        CBX_ShowIgnored.CheckMarkColor = Color.FromArgb(235, 234, 234);
        CBX_ShowIgnored.Cursor = Cursors.Hand;
        CBX_ShowIgnored.Font = new Font("Leelawadee UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        CBX_ShowIgnored.ForeColor = Color.FromArgb(235, 234, 234);
        CBX_ShowIgnored.Location = new Point(484, 495);
        CBX_ShowIgnored.Name = "CBX_ShowIgnored";
        CBX_ShowIgnored.Size = new Size(145, 24);
        CBX_ShowIgnored.TabIndex = 4;
        CBX_ShowIgnored.Text = "Ignored materials";
        TTP_Main.SetToolTip(CBX_ShowIgnored, "Show/Hide ignored materials");
        CBX_ShowIgnored.UncheckedState.BorderColor = Color.FromArgb(103, 99, 99);
        CBX_ShowIgnored.UncheckedState.BorderRadius = 2;
        CBX_ShowIgnored.UncheckedState.BorderThickness = 2;
        CBX_ShowIgnored.UncheckedState.FillColor = Color.FromArgb(58, 55, 55);
        CBX_ShowIgnored.UseVisualStyleBackColor = false;
        CBX_ShowIgnored.CheckedChanged += CBX_ShowIgnored_CheckedChanged;
        // 
        // IBN_About
        // 
        IBN_About.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        IBN_About.CheckedState.ImageSize = new Size(64, 64);
        IBN_About.Cursor = Cursors.Hand;
        IBN_About.HoverState.ImageSize = new Size(25, 25);
        IBN_About.Image = Properties.Resources.About;
        IBN_About.ImageOffset = new Point(0, 0);
        IBN_About.ImageRotate = 0F;
        IBN_About.ImageSize = new Size(25, 25);
        IBN_About.Location = new Point(969, 495);
        IBN_About.Name = "IBN_About";
        IBN_About.PressedState.ImageSize = new Size(25, 25);
        IBN_About.ShadowDecoration.CustomizableEdges = customizableEdges3;
        IBN_About.Size = new Size(30, 30);
        IBN_About.TabIndex = 5;
        TTP_Main.SetToolTip(IBN_About, "About this software");
        IBN_About.Click += IBN_About_Click;
        // 
        // TTP_Main
        // 
        TTP_Main.AutomaticDelay = 200;
        TTP_Main.AutoPopDelay = 99999;
        TTP_Main.BackColor = Color.FromArgb(38, 34, 34);
        TTP_Main.ForeColor = Color.FromArgb(235, 234, 234);
        TTP_Main.InitialDelay = 200;
        TTP_Main.ReshowDelay = 0;
        TTP_Main.ToolTipIcon = ToolTipIcon.Info;
        TTP_Main.ToolTipTitle = "Information";
        // 
        // FRM_MaterialList
        // 
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(38, 34, 34);
        ClientSize = new Size(999, 527);
        Controls.Add(IBN_About);
        Controls.Add(CBX_ShowIgnored);
        Controls.Add(CBX_ShowCollected);
        Controls.Add(CBX_ShowReplaced);
        Controls.Add(CHB_ShowMissing);
        Controls.Add(VSB_Main);
        Controls.Add(PNL_MaterialList);
        Controls.Add(PNL_ProjectInfo);
        Font = new Font("Leelawadee UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
        ForeColor = Color.FromArgb(235, 234, 234);
        Icon = (Icon)resources.GetObject("$this.Icon");
        Margin = new Padding(4);
        MinimumSize = new Size(1015, 566);
        Name = "FRM_MaterialList";
        Opacity = 0D;
        StartPosition = FormStartPosition.CenterScreen;
        Text = "FRM_MaterialList";
        Load += FRM_MaterialList_Load;
        PNL_ProjectInfo.ResumeLayout(false);
        PNL_ProjectInfo.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Guna.UI2.WinForms.Guna2Panel PNL_ProjectInfo;
    private Label LBL_ProjectName;
    private Label LBL_TotalBlocks;
    private Label LBL_DifferentMaterialsCount;
    private FlowLayoutPanel PNL_MaterialList;
    private Guna.UI2.WinForms.Guna2VScrollBar VSB_Main;
    private Guna.UI2.WinForms.Guna2CheckBox CHB_ShowMissing;
    private Guna.UI2.WinForms.Guna2CheckBox CBX_ShowReplaced;
    private Guna.UI2.WinForms.Guna2CheckBox CBX_ShowCollected;
    private Guna.UI2.WinForms.Guna2CheckBox CBX_ShowIgnored;
    private Guna.UI2.WinForms.Guna2ImageButton IBN_About;
    private ToolTip TTP_Main;
}