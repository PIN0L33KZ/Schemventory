namespace Schemventory;

partial class ProjectControl
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
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        PNL_Background = new Guna.UI2.WinForms.Guna2Panel();
        IBN_DeleteProject = new Guna.UI2.WinForms.Guna2ImageButton();
        IBN_EditProject = new Guna.UI2.WinForms.Guna2ImageButton();
        PBX_ProjectIcon = new PictureBox();
        LBL_LastOpened = new Label();
        LBL_CreatedAt = new Label();
        LBL_SchematicPath = new Label();
        LBL_ProjectName = new Label();
        TTP_Main = new ToolTip(components);
        PNL_Background.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)PBX_ProjectIcon).BeginInit();
        SuspendLayout();
        // 
        // PNL_Background
        // 
        PNL_Background.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        PNL_Background.BackColor = Color.Transparent;
        PNL_Background.BorderColor = Color.FromArgb(103, 99, 99);
        PNL_Background.BorderRadius = 5;
        PNL_Background.BorderThickness = 2;
        PNL_Background.Controls.Add(IBN_DeleteProject);
        PNL_Background.Controls.Add(IBN_EditProject);
        PNL_Background.Controls.Add(PBX_ProjectIcon);
        PNL_Background.Controls.Add(LBL_LastOpened);
        PNL_Background.Controls.Add(LBL_CreatedAt);
        PNL_Background.Controls.Add(LBL_SchematicPath);
        PNL_Background.Controls.Add(LBL_ProjectName);
        PNL_Background.CustomizableEdges = customizableEdges3;
        PNL_Background.FillColor = Color.FromArgb(38, 34, 34);
        PNL_Background.Font = new Font("Leelawadee UI", 11.25F);
        PNL_Background.ForeColor = Color.FromArgb(235, 234, 234);
        PNL_Background.Location = new Point(3, 3);
        PNL_Background.Name = "PNL_Background";
        PNL_Background.ShadowDecoration.CustomizableEdges = customizableEdges4;
        PNL_Background.Size = new Size(643, 79);
        PNL_Background.TabIndex = 0;
        PNL_Background.UseTransparentBackground = true;
        // 
        // IBN_DeleteProject
        // 
        IBN_DeleteProject.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        IBN_DeleteProject.CheckedState.ImageSize = new Size(25, 25);
        IBN_DeleteProject.HoverState.ImageSize = new Size(25, 25);
        IBN_DeleteProject.Image = Properties.Resources.DelteProject;
        IBN_DeleteProject.ImageOffset = new Point(0, 0);
        IBN_DeleteProject.ImageRotate = 0F;
        IBN_DeleteProject.ImageSize = new Size(25, 25);
        IBN_DeleteProject.Location = new Point(582, 3);
        IBN_DeleteProject.Name = "IBN_DeleteProject";
        IBN_DeleteProject.PressedState.ImageSize = new Size(25, 25);
        IBN_DeleteProject.ShadowDecoration.CustomizableEdges = customizableEdges1;
        IBN_DeleteProject.Size = new Size(25, 25);
        IBN_DeleteProject.TabIndex = 1;
        TTP_Main.SetToolTip(IBN_DeleteProject, "Remove this project");
        IBN_DeleteProject.UseTransparentBackground = true;
        IBN_DeleteProject.Visible = false;
        IBN_DeleteProject.Click += IBN_DeleteProject_Click;
        // 
        // IBN_EditProject
        // 
        IBN_EditProject.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        IBN_EditProject.CheckedState.ImageSize = new Size(25, 25);
        IBN_EditProject.HoverState.ImageSize = new Size(25, 25);
        IBN_EditProject.Image = Properties.Resources.EditProject;
        IBN_EditProject.ImageOffset = new Point(0, 0);
        IBN_EditProject.ImageRotate = 0F;
        IBN_EditProject.ImageSize = new Size(25, 25);
        IBN_EditProject.Location = new Point(610, 3);
        IBN_EditProject.Name = "IBN_EditProject";
        IBN_EditProject.PressedState.ImageSize = new Size(25, 25);
        IBN_EditProject.ShadowDecoration.CustomizableEdges = customizableEdges2;
        IBN_EditProject.Size = new Size(25, 25);
        IBN_EditProject.TabIndex = 2;
        TTP_Main.SetToolTip(IBN_EditProject, "Edit this project");
        IBN_EditProject.UseTransparentBackground = true;
        IBN_EditProject.Visible = false;
        IBN_EditProject.Click += IBN_EditProject_Click;
        // 
        // PBX_ProjectIcon
        // 
        PBX_ProjectIcon.Image = Properties.Resources.ProjectIconL;
        PBX_ProjectIcon.Location = new Point(8, 6);
        PBX_ProjectIcon.Name = "PBX_ProjectIcon";
        PBX_ProjectIcon.Size = new Size(50, 50);
        PBX_ProjectIcon.SizeMode = PictureBoxSizeMode.AutoSize;
        PBX_ProjectIcon.TabIndex = 9;
        PBX_ProjectIcon.TabStop = false;
        // 
        // LBL_LastOpened
        // 
        LBL_LastOpened.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        LBL_LastOpened.AutoEllipsis = true;
        LBL_LastOpened.Font = new Font("Leelawadee UI", 9F);
        LBL_LastOpened.ForeColor = Color.FromArgb(151, 148, 148);
        LBL_LastOpened.Location = new Point(459, 59);
        LBL_LastOpened.Name = "LBL_LastOpened";
        LBL_LastOpened.Size = new Size(172, 15);
        LBL_LastOpened.TabIndex = 0;
        LBL_LastOpened.Text = "Last opened: 00.00.0000 00:00";
        LBL_LastOpened.TextAlign = ContentAlignment.MiddleRight;
        // 
        // LBL_CreatedAt
        // 
        LBL_CreatedAt.AutoEllipsis = true;
        LBL_CreatedAt.AutoSize = true;
        LBL_CreatedAt.Font = new Font("Leelawadee UI", 9F);
        LBL_CreatedAt.ForeColor = Color.FromArgb(151, 148, 148);
        LBL_CreatedAt.Location = new Point(8, 59);
        LBL_CreatedAt.Name = "LBL_CreatedAt";
        LBL_CreatedAt.Size = new Size(138, 15);
        LBL_CreatedAt.TabIndex = 0;
        LBL_CreatedAt.Text = "Created: 00.00.0000 00:00";
        // 
        // LBL_SchematicPath
        // 
        LBL_SchematicPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        LBL_SchematicPath.AutoEllipsis = true;
        LBL_SchematicPath.Font = new Font("Leelawadee UI", 9F);
        LBL_SchematicPath.ForeColor = Color.FromArgb(151, 148, 148);
        LBL_SchematicPath.Location = new Point(63, 36);
        LBL_SchematicPath.Name = "LBL_SchematicPath";
        LBL_SchematicPath.Size = new Size(567, 20);
        LBL_SchematicPath.TabIndex = 0;
        LBL_SchematicPath.Text = "SchematicPath";
        // 
        // LBL_ProjectName
        // 
        LBL_ProjectName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        LBL_ProjectName.Font = new Font("Leelawadee UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
        LBL_ProjectName.ForeColor = Color.FromArgb(194, 165, 142);
        LBL_ProjectName.Location = new Point(64, 3);
        LBL_ProjectName.Name = "LBL_ProjectName";
        LBL_ProjectName.Size = new Size(509, 33);
        LBL_ProjectName.TabIndex = 0;
        LBL_ProjectName.Text = "ProjectName";
        LBL_ProjectName.TextAlign = ContentAlignment.MiddleLeft;
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
        // ProjectControl
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.Transparent;
        Controls.Add(PNL_Background);
        Cursor = Cursors.Hand;
        Name = "ProjectControl";
        Size = new Size(649, 85);
        Load += ProjectControl_Load;
        PNL_Background.ResumeLayout(false);
        PNL_Background.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)PBX_ProjectIcon).EndInit();
        ResumeLayout(false);
    }

    #endregion

    private Guna.UI2.WinForms.Guna2Panel PNL_Background;
    private Label LBL_ProjectName;
    private Label LBL_SchematicPath;
    private Label LBL_CreatedAt;
    private Label LBL_LastOpened;
    private PictureBox PBX_ProjectIcon;
    private Guna.UI2.WinForms.Guna2ImageButton IBN_EditProject;
    private Guna.UI2.WinForms.Guna2ImageButton IBN_DeleteProject;
    private ToolTip TTP_Main;
}
