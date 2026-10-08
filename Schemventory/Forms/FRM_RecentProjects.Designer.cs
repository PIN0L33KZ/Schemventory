namespace Schemventory;

partial class FRM_RecentProjects
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
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
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FRM_RecentProjects));
        BTN_NewProject = new Guna.UI2.WinForms.Guna2Button();
        BTN_CloseApp = new Guna.UI2.WinForms.Guna2Button();
        PBX_AppLogo = new PictureBox();
        LBL_AppTitle = new Label();
        PNL_RecentProjectControls = new FlowLayoutPanel();
        VSB_Main = new Guna.UI2.WinForms.Guna2VScrollBar();
        LBL_NoProjectsWarn = new Label();
        ((System.ComponentModel.ISupportInitialize)PBX_AppLogo).BeginInit();
        SuspendLayout();
        // 
        // BTN_NewProject
        // 
        BTN_NewProject.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        BTN_NewProject.Animated = true;
        BTN_NewProject.BackColor = Color.Transparent;
        BTN_NewProject.BorderRadius = 5;
        BTN_NewProject.Cursor = Cursors.Hand;
        BTN_NewProject.CustomizableEdges = customizableEdges1;
        BTN_NewProject.DialogResult = DialogResult.OK;
        BTN_NewProject.DisabledState.BorderColor = Color.FromArgb(80, 76, 76);
        BTN_NewProject.DisabledState.CustomBorderColor = Color.FromArgb(80, 76, 76);
        BTN_NewProject.DisabledState.FillColor = Color.FromArgb(80, 76, 76);
        BTN_NewProject.DisabledState.ForeColor = Color.FromArgb(151, 148, 148);
        BTN_NewProject.FillColor = Color.FromArgb(162, 123, 90);
        BTN_NewProject.Font = new Font("Leelawadee UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        BTN_NewProject.ForeColor = Color.White;
        BTN_NewProject.Location = new Point(276, 581);
        BTN_NewProject.Name = "BTN_NewProject";
        BTN_NewProject.ShadowDecoration.CustomizableEdges = customizableEdges2;
        BTN_NewProject.Size = new Size(119, 32);
        BTN_NewProject.TabIndex = 3;
        BTN_NewProject.Text = "New project";
        BTN_NewProject.UseTransparentBackground = true;
        BTN_NewProject.Click += BTN_NewProject_Click;
        // 
        // BTN_CloseApp
        // 
        BTN_CloseApp.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        BTN_CloseApp.Animated = true;
        BTN_CloseApp.BackColor = Color.Transparent;
        BTN_CloseApp.BorderColor = Color.FromArgb(80, 76, 76);
        BTN_CloseApp.BorderRadius = 5;
        BTN_CloseApp.BorderThickness = 2;
        BTN_CloseApp.Cursor = Cursors.Hand;
        BTN_CloseApp.CustomizableEdges = customizableEdges3;
        BTN_CloseApp.DialogResult = DialogResult.OK;
        BTN_CloseApp.DisabledState.BorderColor = Color.FromArgb(80, 76, 76);
        BTN_CloseApp.DisabledState.CustomBorderColor = Color.FromArgb(80, 76, 76);
        BTN_CloseApp.DisabledState.FillColor = Color.FromArgb(80, 76, 76);
        BTN_CloseApp.DisabledState.ForeColor = Color.FromArgb(151, 148, 148);
        BTN_CloseApp.FillColor = Color.FromArgb(80, 76, 76);
        BTN_CloseApp.Font = new Font("Leelawadee UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        BTN_CloseApp.ForeColor = Color.White;
        BTN_CloseApp.HoverState.BorderColor = Color.FromArgb(103, 99, 99);
        BTN_CloseApp.Location = new Point(12, 581);
        BTN_CloseApp.Name = "BTN_CloseApp";
        BTN_CloseApp.ShadowDecoration.CustomizableEdges = customizableEdges4;
        BTN_CloseApp.Size = new Size(119, 32);
        BTN_CloseApp.TabIndex = 2;
        BTN_CloseApp.Text = "Close app";
        BTN_CloseApp.UseTransparentBackground = true;
        BTN_CloseApp.Click += BTN_CloseApp_Click;
        // 
        // PBX_AppLogo
        // 
        PBX_AppLogo.Anchor = AnchorStyles.Top;
        PBX_AppLogo.Image = Properties.Resources.AppLogo;
        PBX_AppLogo.Location = new Point(153, 13);
        PBX_AppLogo.Name = "PBX_AppLogo";
        PBX_AppLogo.Size = new Size(100, 100);
        PBX_AppLogo.SizeMode = PictureBoxSizeMode.AutoSize;
        PBX_AppLogo.TabIndex = 3;
        PBX_AppLogo.TabStop = false;
        // 
        // LBL_AppTitle
        // 
        LBL_AppTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        LBL_AppTitle.Font = new Font("Leelawadee UI", 20.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        LBL_AppTitle.ForeColor = Color.FromArgb(162, 123, 90);
        LBL_AppTitle.Location = new Point(8, 116);
        LBL_AppTitle.Name = "LBL_AppTitle";
        LBL_AppTitle.Size = new Size(390, 45);
        LBL_AppTitle.TabIndex = 0;
        LBL_AppTitle.Text = "AppName";
        LBL_AppTitle.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // PNL_RecentProjectControls
        // 
        PNL_RecentProjectControls.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        PNL_RecentProjectControls.AutoScroll = true;
        PNL_RecentProjectControls.Location = new Point(12, 164);
        PNL_RecentProjectControls.Name = "PNL_RecentProjectControls";
        PNL_RecentProjectControls.Size = new Size(383, 411);
        PNL_RecentProjectControls.TabIndex = 1;
        // 
        // VSB_Main
        // 
        VSB_Main.BackColor = Color.Transparent;
        VSB_Main.BindingContainer = PNL_RecentProjectControls;
        VSB_Main.BorderRadius = 5;
        VSB_Main.FillColor = Color.FromArgb(80, 76, 76);
        VSB_Main.InUpdate = false;
        VSB_Main.LargeChange = 10;
        VSB_Main.Location = new Point(377, 164);
        VSB_Main.Name = "VSB_Main";
        VSB_Main.ScrollbarSize = 18;
        VSB_Main.Size = new Size(18, 411);
        VSB_Main.TabIndex = 0;
        VSB_Main.TabStop = false;
        VSB_Main.ThumbColor = Color.FromArgb(103, 99, 99);
        VSB_Main.ThumbSize = 5F;
        VSB_Main.ThumbStyle = Guna.UI2.WinForms.Enums.ThumbStyle.Inset;
        // 
        // LBL_NoProjectsWarn
        // 
        LBL_NoProjectsWarn.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        LBL_NoProjectsWarn.AutoEllipsis = true;
        LBL_NoProjectsWarn.Font = new Font("Leelawadee UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        LBL_NoProjectsWarn.ForeColor = Color.FromArgb(235, 234, 234);
        LBL_NoProjectsWarn.Location = new Point(12, 161);
        LBL_NoProjectsWarn.Name = "LBL_NoProjectsWarn";
        LBL_NoProjectsWarn.Size = new Size(383, 414);
        LBL_NoProjectsWarn.TabIndex = 7;
        LBL_NoProjectsWarn.Text = "No projects. Create a new project!";
        LBL_NoProjectsWarn.TextAlign = ContentAlignment.MiddleCenter;
        LBL_NoProjectsWarn.Visible = false;
        // 
        // FRM_RecentProjects
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(38, 34, 34);
        ClientSize = new Size(407, 625);
        Controls.Add(VSB_Main);
        Controls.Add(LBL_AppTitle);
        Controls.Add(PBX_AppLogo);
        Controls.Add(BTN_CloseApp);
        Controls.Add(BTN_NewProject);
        Controls.Add(PNL_RecentProjectControls);
        Controls.Add(LBL_NoProjectsWarn);
        Font = new Font("Leelawadee UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        ForeColor = Color.FromArgb(235, 234, 234);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        Icon = (Icon)resources.GetObject("$this.Icon");
        Margin = new Padding(3, 4, 3, 4);
        MaximizeBox = false;
        Name = "FRM_RecentProjects";
        StartPosition = FormStartPosition.CenterScreen;
        Load += FRM_RecentProjects_Load;
        ((System.ComponentModel.ISupportInitialize)PBX_AppLogo).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Guna.UI2.WinForms.Guna2Button BTN_NewProject;
    private Guna.UI2.WinForms.Guna2Button BTN_CloseApp;
    private PictureBox PBX_AppLogo;
    private Label LBL_AppTitle;
    private FlowLayoutPanel PNL_RecentProjectControls;
    private Guna.UI2.WinForms.Guna2VScrollBar VSB_Main;
    private Label LBL_NoProjectsWarn;
}
