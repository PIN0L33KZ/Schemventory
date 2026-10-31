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
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FRM_MaterialList));
        PNL_ProjectInfo = new Guna.UI2.WinForms.Guna2Panel();
        LBL_DifferentMaterialsCount = new Label();
        LBL_TotalBlocks = new Label();
        LBL_ProjectName = new Label();
        PNL_MaterialList = new Schemventory.Controls.VirtualMaterialPanel();
        VSB_Main = new Guna.UI2.WinForms.Guna2VScrollBar();
        IBN_About = new Guna.UI2.WinForms.Guna2ImageButton();
        TTP_Main = new ToolTip(components);
        LBL_FilterWarn = new Label();
        BTN_AddMaterial = new Guna.UI2.WinForms.Guna2Button();
        FPN_Ignored = new Schemventory.Controls.FilterPanel();
        FPN_Collected = new Schemventory.Controls.FilterPanel();
        FPN_Replaced = new Schemventory.Controls.FilterPanel();
        FPN_Missing = new Schemventory.Controls.FilterPanel();
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
        PNL_MaterialList.BackColor = Color.Transparent;
        PNL_MaterialList.Location = new Point(12, 128);
        PNL_MaterialList.Name = "PNL_MaterialList";
        PNL_MaterialList.Size = new Size(967, 415);
        PNL_MaterialList.TabIndex = 0;
        PNL_MaterialList.TabStop = true;
        // 
        // VSB_Main
        // 
        VSB_Main.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
        VSB_Main.BackColor = Color.Transparent;
        VSB_Main.BorderRadius = 5;
        VSB_Main.FillColor = Color.FromArgb(80, 76, 76);
        VSB_Main.InUpdate = false;
        VSB_Main.LargeChange = 10;
        VSB_Main.Location = new Point(981, 128);
        VSB_Main.Name = "VSB_Main";
        VSB_Main.ScrollbarSize = 18;
        VSB_Main.Size = new Size(18, 415);
        VSB_Main.TabIndex = 0;
        VSB_Main.TabStop = false;
        VSB_Main.ThumbColor = Color.FromArgb(103, 99, 99);
        VSB_Main.ThumbSize = 5F;
        VSB_Main.ThumbStyle = Guna.UI2.WinForms.Enums.ThumbStyle.Inset;
        VSB_Main.Visible = false;
        VSB_Main.Scroll += VSB_Main_Scroll;
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
        IBN_About.Location = new Point(969, 549);
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
        // LBL_FilterWarn
        // 
        LBL_FilterWarn.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        LBL_FilterWarn.AutoEllipsis = true;
        LBL_FilterWarn.Font = new Font("Leelawadee UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        LBL_FilterWarn.ForeColor = Color.FromArgb(235, 234, 234);
        LBL_FilterWarn.Location = new Point(12, 125);
        LBL_FilterWarn.Name = "LBL_FilterWarn";
        LBL_FilterWarn.Size = new Size(975, 421);
        LBL_FilterWarn.TabIndex = 6;
        LBL_FilterWarn.Text = "No materials to display based on the current filters.\r\nTry a different filter!";
        LBL_FilterWarn.TextAlign = ContentAlignment.MiddleCenter;
        LBL_FilterWarn.Visible = false;
        // 
        // BTN_AddMaterial
        // 
        BTN_AddMaterial.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        BTN_AddMaterial.Animated = true;
        BTN_AddMaterial.BackColor = Color.Transparent;
        BTN_AddMaterial.BorderRadius = 5;
        BTN_AddMaterial.Cursor = Cursors.Hand;
        BTN_AddMaterial.CustomizableEdges = customizableEdges4;
        BTN_AddMaterial.DisabledState.BorderColor = Color.FromArgb(80, 76, 76);
        BTN_AddMaterial.DisabledState.CustomBorderColor = Color.FromArgb(80, 76, 76);
        BTN_AddMaterial.DisabledState.FillColor = Color.FromArgb(80, 76, 76);
        BTN_AddMaterial.DisabledState.ForeColor = Color.FromArgb(151, 148, 148);
        BTN_AddMaterial.FillColor = Color.FromArgb(162, 123, 90);
        BTN_AddMaterial.Font = new Font("Leelawadee UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        BTN_AddMaterial.ForeColor = Color.White;
        BTN_AddMaterial.Location = new Point(861, 74);
        BTN_AddMaterial.Name = "BTN_AddMaterial";
        BTN_AddMaterial.ShadowDecoration.CustomizableEdges = customizableEdges5;
        BTN_AddMaterial.Size = new Size(126, 47);
        BTN_AddMaterial.TabIndex = 7;
        BTN_AddMaterial.Text = "Add material";
        BTN_AddMaterial.UseTransparentBackground = true;
        BTN_AddMaterial.Click += BTN_AddMaterial_Click;
        // 
        // FPN_Ignored
        // 
        FPN_Ignored.BackColor = Color.Transparent;
        FPN_Ignored.FilterStyle = Schemventory.Controls.FilterStyle.Ignored;
        FPN_Ignored.Font = new Font("Leelawadee UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        FPN_Ignored.ForeColor = Color.FromArgb(235, 234, 234);
        FPN_Ignored.Location = new Point(483, 74);
        FPN_Ignored.Margin = new Padding(3, 4, 3, 4);
        FPN_Ignored.Name = "FPN_Ignored";
        FPN_Ignored.Size = new Size(146, 47);
        FPN_Ignored.TabIndex = 0;
        // 
        // FPN_Collected
        // 
        FPN_Collected.BackColor = Color.Transparent;
        FPN_Collected.FilterStyle = Schemventory.Controls.FilterStyle.Collected;
        FPN_Collected.Font = new Font("Leelawadee UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        FPN_Collected.ForeColor = Color.FromArgb(235, 234, 234);
        FPN_Collected.Location = new Point(322, 74);
        FPN_Collected.Margin = new Padding(3, 4, 3, 4);
        FPN_Collected.Name = "FPN_Collected";
        FPN_Collected.Size = new Size(155, 47);
        FPN_Collected.TabIndex = 0;
        // 
        // FPN_Replaced
        // 
        FPN_Replaced.BackColor = Color.Transparent;
        FPN_Replaced.FilterStyle = Schemventory.Controls.FilterStyle.Replaced;
        FPN_Replaced.Font = new Font("Leelawadee UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        FPN_Replaced.ForeColor = Color.FromArgb(235, 234, 234);
        FPN_Replaced.Location = new Point(163, 74);
        FPN_Replaced.Margin = new Padding(3, 4, 3, 4);
        FPN_Replaced.Name = "FPN_Replaced";
        FPN_Replaced.Size = new Size(153, 47);
        FPN_Replaced.TabIndex = 0;
        // 
        // FPN_Missing
        // 
        FPN_Missing.BackColor = Color.Transparent;
        FPN_Missing.Font = new Font("Leelawadee UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        FPN_Missing.ForeColor = Color.FromArgb(235, 234, 234);
        FPN_Missing.Location = new Point(12, 74);
        FPN_Missing.Margin = new Padding(3, 4, 3, 4);
        FPN_Missing.Name = "FPN_Missing";
        FPN_Missing.Size = new Size(145, 47);
        FPN_Missing.TabIndex = 0;
        // 
        // FRM_MaterialList
        // 
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(38, 34, 34);
        ClientSize = new Size(999, 581);
        Controls.Add(BTN_AddMaterial);
        Controls.Add(FPN_Ignored);
        Controls.Add(VSB_Main);
        Controls.Add(FPN_Collected);
        Controls.Add(IBN_About);
        Controls.Add(FPN_Replaced);
        Controls.Add(FPN_Missing);
        Controls.Add(PNL_ProjectInfo);
        Controls.Add(PNL_MaterialList);
        Controls.Add(LBL_FilterWarn);
        DoubleBuffered = true;
        Font = new Font("Leelawadee UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
        ForeColor = Color.FromArgb(235, 234, 234);
        Icon = (Icon)resources.GetObject("$this.Icon");
        Margin = new Padding(4);
        MinimumSize = new Size(1015, 615);
        Name = "FRM_MaterialList";
        Opacity = 0D;
        StartPosition = FormStartPosition.CenterScreen;
        Text = "FRM_MaterialList";
        Load += FRM_MaterialList_Load;
        ResizeBegin += FRM_MaterialList_ResizeBegin;
        ResizeEnd += FRM_MaterialList_ResizeEnd;
        PNL_ProjectInfo.ResumeLayout(false);
        PNL_ProjectInfo.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private Guna.UI2.WinForms.Guna2Panel PNL_ProjectInfo;
    private Label LBL_ProjectName;
    private Label LBL_TotalBlocks;
    private Label LBL_DifferentMaterialsCount;
    private Schemventory.Controls.VirtualMaterialPanel PNL_MaterialList;
    private Guna.UI2.WinForms.Guna2VScrollBar VSB_Main;
    private Guna.UI2.WinForms.Guna2ImageButton IBN_About;
    private ToolTip TTP_Main;
    private Label LBL_FilterWarn;
    private Guna.UI2.WinForms.Guna2Button BTN_AddMaterial;
    private Controls.FilterPanel FPN_Missing;
    private Controls.FilterPanel FPN_Replaced;
    private Controls.FilterPanel FPN_Ignored;
    private Controls.FilterPanel FPN_Collected;
}
