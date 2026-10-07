namespace Schemventory.Forms;

partial class FRM_NewProject
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
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges9 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges10 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FRM_NewProject));
        LBL_ProjectName = new Label();
        TBX_ProjectName = new Guna.UI2.WinForms.Guna2TextBox();
        TBX_SchematicFilePath = new Guna.UI2.WinForms.Guna2TextBox();
        BTN_SelectSchematicFilePath = new Guna.UI2.WinForms.Guna2Button();
        LBL_SchematicFile = new Label();
        BTN_CreateProject = new Guna.UI2.WinForms.Guna2Button();
        BTN_Cancel = new Guna.UI2.WinForms.Guna2Button();
        SuspendLayout();
        // 
        // LBL_ProjectName
        // 
        LBL_ProjectName.AutoSize = true;
        LBL_ProjectName.Font = new Font("Leelawadee UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        LBL_ProjectName.ForeColor = Color.FromArgb(235, 234, 234);
        LBL_ProjectName.Location = new Point(12, 28);
        LBL_ProjectName.Name = "LBL_ProjectName";
        LBL_ProjectName.Size = new Size(99, 20);
        LBL_ProjectName.TabIndex = 0;
        LBL_ProjectName.Text = "Project name:";
        // 
        // TBX_ProjectName
        // 
        TBX_ProjectName.Animated = true;
        TBX_ProjectName.BackColor = Color.FromArgb(38, 34, 34);
        TBX_ProjectName.BorderColor = Color.FromArgb(103, 99, 99);
        TBX_ProjectName.BorderRadius = 5;
        TBX_ProjectName.BorderThickness = 2;
        TBX_ProjectName.CustomizableEdges = customizableEdges1;
        TBX_ProjectName.DefaultText = "";
        TBX_ProjectName.DisabledState.BorderColor = Color.FromArgb(80, 76, 76);
        TBX_ProjectName.DisabledState.FillColor = Color.FromArgb(80, 76, 76);
        TBX_ProjectName.DisabledState.ForeColor = Color.FromArgb(151, 148, 148);
        TBX_ProjectName.DisabledState.PlaceholderForeColor = Color.FromArgb(80, 76, 76);
        TBX_ProjectName.FillColor = Color.FromArgb(58, 55, 55);
        TBX_ProjectName.FocusedState.BorderColor = Color.FromArgb(162, 123, 90);
        TBX_ProjectName.Font = new Font("Leelawadee UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        TBX_ProjectName.ForeColor = Color.FromArgb(235, 234, 234);
        TBX_ProjectName.HoverState.BorderColor = Color.FromArgb(194, 165, 142);
        TBX_ProjectName.IconLeft = Properties.Resources.Input;
        TBX_ProjectName.IconLeftSize = new Size(25, 25);
        TBX_ProjectName.Location = new Point(12, 52);
        TBX_ProjectName.Margin = new Padding(3, 4, 3, 4);
        TBX_ProjectName.Name = "TBX_ProjectName";
        TBX_ProjectName.PlaceholderForeColor = Color.FromArgb(151, 148, 148);
        TBX_ProjectName.PlaceholderText = "House schematic";
        TBX_ProjectName.SelectedText = "";
        TBX_ProjectName.ShadowDecoration.CustomizableEdges = customizableEdges2;
        TBX_ProjectName.Size = new Size(230, 33);
        TBX_ProjectName.TabIndex = 1;
        // 
        // TBX_SchematicFilePath
        // 
        TBX_SchematicFilePath.Animated = true;
        TBX_SchematicFilePath.AutoScroll = true;
        TBX_SchematicFilePath.BackColor = Color.FromArgb(38, 34, 34);
        TBX_SchematicFilePath.BorderColor = Color.FromArgb(103, 99, 99);
        TBX_SchematicFilePath.BorderRadius = 5;
        TBX_SchematicFilePath.BorderThickness = 2;
        TBX_SchematicFilePath.CustomizableEdges = customizableEdges3;
        TBX_SchematicFilePath.DefaultText = "";
        TBX_SchematicFilePath.DisabledState.BorderColor = Color.FromArgb(80, 76, 76);
        TBX_SchematicFilePath.DisabledState.FillColor = Color.FromArgb(80, 76, 76);
        TBX_SchematicFilePath.DisabledState.ForeColor = Color.FromArgb(151, 148, 148);
        TBX_SchematicFilePath.DisabledState.PlaceholderForeColor = Color.FromArgb(80, 76, 76);
        TBX_SchematicFilePath.FillColor = Color.FromArgb(58, 55, 55);
        TBX_SchematicFilePath.FocusedState.BorderColor = Color.FromArgb(162, 123, 90);
        TBX_SchematicFilePath.Font = new Font("Leelawadee UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        TBX_SchematicFilePath.ForeColor = Color.FromArgb(235, 234, 234);
        TBX_SchematicFilePath.HoverState.BorderColor = Color.FromArgb(194, 165, 142);
        TBX_SchematicFilePath.IconLeft = Properties.Resources.Filepath;
        TBX_SchematicFilePath.IconLeftSize = new Size(25, 25);
        TBX_SchematicFilePath.Location = new Point(12, 123);
        TBX_SchematicFilePath.Margin = new Padding(3, 5, 3, 5);
        TBX_SchematicFilePath.Name = "TBX_SchematicFilePath";
        TBX_SchematicFilePath.PlaceholderForeColor = Color.FromArgb(151, 148, 148);
        TBX_SchematicFilePath.PlaceholderText = "No schematic file selected";
        TBX_SchematicFilePath.ReadOnly = true;
        TBX_SchematicFilePath.SelectedText = "";
        TBX_SchematicFilePath.ShadowDecoration.CustomizableEdges = customizableEdges4;
        TBX_SchematicFilePath.Size = new Size(413, 33);
        TBX_SchematicFilePath.TabIndex = 2;
        // 
        // BTN_SelectSchematicFilePath
        // 
        BTN_SelectSchematicFilePath.Animated = true;
        BTN_SelectSchematicFilePath.BackColor = Color.Transparent;
        BTN_SelectSchematicFilePath.BorderColor = Color.FromArgb(80, 76, 76);
        BTN_SelectSchematicFilePath.BorderRadius = 5;
        BTN_SelectSchematicFilePath.BorderThickness = 2;
        BTN_SelectSchematicFilePath.Cursor = Cursors.Hand;
        BTN_SelectSchematicFilePath.CustomizableEdges = customizableEdges5;
        BTN_SelectSchematicFilePath.DisabledState.BorderColor = Color.FromArgb(80, 76, 76);
        BTN_SelectSchematicFilePath.DisabledState.CustomBorderColor = Color.FromArgb(80, 76, 76);
        BTN_SelectSchematicFilePath.DisabledState.FillColor = Color.FromArgb(80, 76, 76);
        BTN_SelectSchematicFilePath.DisabledState.ForeColor = Color.FromArgb(151, 148, 148);
        BTN_SelectSchematicFilePath.FillColor = Color.FromArgb(80, 76, 76);
        BTN_SelectSchematicFilePath.Font = new Font("Leelawadee UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        BTN_SelectSchematicFilePath.ForeColor = Color.White;
        BTN_SelectSchematicFilePath.HoverState.BorderColor = Color.FromArgb(103, 99, 99);
        BTN_SelectSchematicFilePath.Location = new Point(431, 123);
        BTN_SelectSchematicFilePath.Name = "BTN_SelectSchematicFilePath";
        BTN_SelectSchematicFilePath.ShadowDecoration.CustomizableEdges = customizableEdges6;
        BTN_SelectSchematicFilePath.Size = new Size(71, 33);
        BTN_SelectSchematicFilePath.TabIndex = 3;
        BTN_SelectSchematicFilePath.Text = "Select";
        BTN_SelectSchematicFilePath.UseTransparentBackground = true;
        BTN_SelectSchematicFilePath.Click += BTN_SelectSchematicFilePath_Click;
        // 
        // LBL_SchematicFile
        // 
        LBL_SchematicFile.AutoSize = true;
        LBL_SchematicFile.Font = new Font("Leelawadee UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        LBL_SchematicFile.ForeColor = Color.FromArgb(235, 234, 234);
        LBL_SchematicFile.Location = new Point(12, 99);
        LBL_SchematicFile.Name = "LBL_SchematicFile";
        LBL_SchematicFile.Size = new Size(105, 20);
        LBL_SchematicFile.TabIndex = 0;
        LBL_SchematicFile.Text = "Schematic file:";
        // 
        // BTN_CreateProject
        // 
        BTN_CreateProject.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        BTN_CreateProject.Animated = true;
        BTN_CreateProject.BackColor = Color.Transparent;
        BTN_CreateProject.BorderRadius = 5;
        BTN_CreateProject.Cursor = Cursors.Hand;
        BTN_CreateProject.CustomizableEdges = customizableEdges7;
        BTN_CreateProject.DialogResult = DialogResult.OK;
        BTN_CreateProject.DisabledState.BorderColor = Color.FromArgb(80, 76, 76);
        BTN_CreateProject.DisabledState.CustomBorderColor = Color.FromArgb(80, 76, 76);
        BTN_CreateProject.DisabledState.FillColor = Color.FromArgb(80, 76, 76);
        BTN_CreateProject.DisabledState.ForeColor = Color.FromArgb(151, 148, 148);
        BTN_CreateProject.FillColor = Color.FromArgb(162, 123, 90);
        BTN_CreateProject.Font = new Font("Leelawadee UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        BTN_CreateProject.ForeColor = Color.White;
        BTN_CreateProject.Location = new Point(364, 196);
        BTN_CreateProject.Name = "BTN_CreateProject";
        BTN_CreateProject.ShadowDecoration.CustomizableEdges = customizableEdges8;
        BTN_CreateProject.Size = new Size(144, 32);
        BTN_CreateProject.TabIndex = 5;
        BTN_CreateProject.Text = "Create project";
        BTN_CreateProject.UseTransparentBackground = true;
        BTN_CreateProject.Click += BTN_CreateProject_Click;
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
        BTN_Cancel.CustomizableEdges = customizableEdges9;
        BTN_Cancel.DialogResult = DialogResult.OK;
        BTN_Cancel.DisabledState.BorderColor = Color.FromArgb(80, 76, 76);
        BTN_Cancel.DisabledState.CustomBorderColor = Color.FromArgb(80, 76, 76);
        BTN_Cancel.DisabledState.FillColor = Color.FromArgb(80, 76, 76);
        BTN_Cancel.DisabledState.ForeColor = Color.FromArgb(151, 148, 148);
        BTN_Cancel.FillColor = Color.FromArgb(80, 76, 76);
        BTN_Cancel.Font = new Font("Leelawadee UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        BTN_Cancel.ForeColor = Color.White;
        BTN_Cancel.HoverState.BorderColor = Color.FromArgb(103, 99, 99);
        BTN_Cancel.Location = new Point(12, 196);
        BTN_Cancel.Name = "BTN_Cancel";
        BTN_Cancel.ShadowDecoration.CustomizableEdges = customizableEdges10;
        BTN_Cancel.Size = new Size(119, 32);
        BTN_Cancel.TabIndex = 4;
        BTN_Cancel.Text = "Cancel";
        BTN_Cancel.UseTransparentBackground = true;
        BTN_Cancel.Click += BTN_Cancel_Click;
        // 
        // FRM_NewProject
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(38, 34, 34);
        ClientSize = new Size(520, 240);
        Controls.Add(BTN_Cancel);
        Controls.Add(BTN_CreateProject);
        Controls.Add(TBX_SchematicFilePath);
        Controls.Add(BTN_SelectSchematicFilePath);
        Controls.Add(TBX_ProjectName);
        Controls.Add(LBL_SchematicFile);
        Controls.Add(LBL_ProjectName);
        Font = new Font("Leelawadee UI", 11.25F);
        ForeColor = Color.FromArgb(235, 234, 234);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        Icon = (Icon)resources.GetObject("$this.Icon");
        Margin = new Padding(3, 4, 3, 4);
        MaximizeBox = false;
        Name = "FRM_NewProject";
        StartPosition = FormStartPosition.CenterScreen;
        Load += FRM_NewProject_Load;
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label LBL_ProjectName;
    private Guna.UI2.WinForms.Guna2TextBox TBX_ProjectName;
    private Guna.UI2.WinForms.Guna2TextBox TBX_SchematicFilePath;
    private Guna.UI2.WinForms.Guna2Button BTN_SelectSchematicFilePath;
    private Label LBL_SchematicFile;
    private Guna.UI2.WinForms.Guna2Button BTN_CreateProject;
    private Guna.UI2.WinForms.Guna2Button BTN_Cancel;
}