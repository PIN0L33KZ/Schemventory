namespace Schemventory.Forms;

partial class FRM_ReplaceMaterial
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
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges11 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges12 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FRM_ReplaceMaterial));
        LBL_Heading = new Label();
        LBL_Description = new Label();
        TBX_Search = new Guna.UI2.WinForms.Guna2TextBox();
        LST_Items = new ListView();
        PNL_Preview = new Guna.UI2.WinForms.Guna2Panel();
        LBL_StackSize = new Label();
        LBL_ItemId = new Label();
        PBX_Icon = new PictureBox();
        LBL_ItemName = new Label();
        PNL_ItemsBackground = new Guna.UI2.WinForms.Guna2Panel();
        VSB_Main = new Guna.UI2.WinForms.Guna2VScrollBar();
        BTN_Replace = new Guna.UI2.WinForms.Guna2Button();
        BTN_Cancel = new Guna.UI2.WinForms.Guna2Button();
        BTN_Reset = new Guna.UI2.WinForms.Guna2Button();
        PNL_Preview.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)PBX_Icon).BeginInit();
        PNL_ItemsBackground.SuspendLayout();
        SuspendLayout();
        // 
        // LBL_Heading
        // 
        LBL_Heading.AutoSize = true;
        LBL_Heading.Font = new Font("Leelawadee UI", 20.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        LBL_Heading.ForeColor = Color.FromArgb(162, 123, 90);
        LBL_Heading.Location = new Point(12, 9);
        LBL_Heading.Name = "LBL_Heading";
        LBL_Heading.Size = new Size(233, 37);
        LBL_Heading.TabIndex = 0;
        LBL_Heading.Text = "Replace material";
        LBL_Heading.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // LBL_Description
        // 
        LBL_Description.AutoSize = true;
        LBL_Description.Font = new Font("Leelawadee UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
        LBL_Description.ForeColor = Color.FromArgb(194, 165, 142);
        LBL_Description.Location = new Point(12, 55);
        LBL_Description.Name = "LBL_Description";
        LBL_Description.Size = new Size(408, 21);
        LBL_Description.TabIndex = 0;
        LBL_Description.Text = "Choose a material to replace $OriginalMaterial with.";
        LBL_Description.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // TBX_Search
        // 
        TBX_Search.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        TBX_Search.Animated = true;
        TBX_Search.BackColor = Color.FromArgb(38, 34, 34);
        TBX_Search.BorderColor = Color.FromArgb(103, 99, 99);
        TBX_Search.BorderRadius = 5;
        TBX_Search.BorderThickness = 2;
        TBX_Search.CustomizableEdges = customizableEdges1;
        TBX_Search.DefaultText = "";
        TBX_Search.DisabledState.BorderColor = Color.FromArgb(80, 76, 76);
        TBX_Search.DisabledState.FillColor = Color.FromArgb(80, 76, 76);
        TBX_Search.DisabledState.ForeColor = Color.FromArgb(151, 148, 148);
        TBX_Search.DisabledState.PlaceholderForeColor = Color.FromArgb(80, 76, 76);
        TBX_Search.FillColor = Color.FromArgb(58, 55, 55);
        TBX_Search.FocusedState.BorderColor = Color.FromArgb(162, 123, 90);
        TBX_Search.Font = new Font("Leelawadee UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        TBX_Search.ForeColor = Color.FromArgb(235, 234, 234);
        TBX_Search.HoverState.BorderColor = Color.FromArgb(194, 165, 142);
        TBX_Search.IconLeft = Properties.Resources.Search;
        TBX_Search.IconLeftSize = new Size(25, 25);
        TBX_Search.Location = new Point(12, 115);
        TBX_Search.Margin = new Padding(3, 4, 3, 4);
        TBX_Search.Name = "TBX_Search";
        TBX_Search.PlaceholderForeColor = Color.FromArgb(151, 148, 148);
        TBX_Search.PlaceholderText = "Search by name or item ID...";
        TBX_Search.SelectedText = "";
        TBX_Search.ShadowDecoration.CustomizableEdges = customizableEdges2;
        TBX_Search.Size = new Size(814, 33);
        TBX_Search.TabIndex = 1;
        TBX_Search.TextChanged += TBX_Search_TextChanged;
        // 
        // LST_Items
        // 
        LST_Items.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        LST_Items.BackColor = Color.FromArgb(58, 55, 55);
        LST_Items.BorderStyle = BorderStyle.None;
        LST_Items.ForeColor = Color.FromArgb(235, 234, 234);
        LST_Items.FullRowSelect = true;
        LST_Items.HeaderStyle = ColumnHeaderStyle.None;
        LST_Items.Location = new Point(3, 3);
        LST_Items.MultiSelect = false;
        LST_Items.Name = "LST_Items";
        LST_Items.Size = new Size(571, 281);
        LST_Items.Sorting = SortOrder.Descending;
        LST_Items.TabIndex = 0;
        LST_Items.UseCompatibleStateImageBehavior = false;
        LST_Items.View = View.Details;
        LST_Items.VirtualMode = true;
        LST_Items.SelectedIndexChanged += LST_Items_SelectedIndexChanged;
        LST_Items.DoubleClick += LST_Items_DoubleClick;
        // 
        // PNL_Preview
        // 
        PNL_Preview.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
        PNL_Preview.BackColor = Color.Transparent;
        PNL_Preview.BorderColor = Color.FromArgb(103, 99, 99);
        PNL_Preview.BorderRadius = 5;
        PNL_Preview.BorderThickness = 2;
        PNL_Preview.Controls.Add(LBL_StackSize);
        PNL_Preview.Controls.Add(LBL_ItemId);
        PNL_Preview.Controls.Add(PBX_Icon);
        PNL_Preview.Controls.Add(LBL_ItemName);
        PNL_Preview.CustomizableEdges = customizableEdges3;
        PNL_Preview.FillColor = Color.FromArgb(58, 55, 55);
        PNL_Preview.Location = new Point(595, 155);
        PNL_Preview.Name = "PNL_Preview";
        PNL_Preview.ShadowDecoration.CustomizableEdges = customizableEdges4;
        PNL_Preview.Size = new Size(231, 287);
        PNL_Preview.TabIndex = 0;
        PNL_Preview.UseTransparentBackground = true;
        // 
        // LBL_StackSize
        // 
        LBL_StackSize.AutoEllipsis = true;
        LBL_StackSize.Font = new Font("Leelawadee UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
        LBL_StackSize.ForeColor = Color.FromArgb(151, 148, 148);
        LBL_StackSize.Location = new Point(14, 198);
        LBL_StackSize.Name = "LBL_StackSize";
        LBL_StackSize.Size = new Size(203, 15);
        LBL_StackSize.TabIndex = 0;
        LBL_StackSize.Text = "Stack size: 0";
        LBL_StackSize.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // LBL_ItemId
        // 
        LBL_ItemId.AutoEllipsis = true;
        LBL_ItemId.Font = new Font("Leelawadee UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
        LBL_ItemId.ForeColor = Color.FromArgb(151, 148, 148);
        LBL_ItemId.Location = new Point(14, 183);
        LBL_ItemId.Name = "LBL_ItemId";
        LBL_ItemId.Size = new Size(203, 15);
        LBL_ItemId.TabIndex = 0;
        LBL_ItemId.Text = "minecraft:empty";
        LBL_ItemId.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // PBX_Icon
        // 
        PBX_Icon.Location = new Point(83, 74);
        PBX_Icon.Name = "PBX_Icon";
        PBX_Icon.Size = new Size(64, 64);
        PBX_Icon.TabIndex = 0;
        PBX_Icon.TabStop = false;
        // 
        // LBL_ItemName
        // 
        LBL_ItemName.AutoEllipsis = true;
        LBL_ItemName.Font = new Font("Leelawadee UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        LBL_ItemName.ForeColor = Color.FromArgb(235, 234, 234);
        LBL_ItemName.Location = new Point(14, 154);
        LBL_ItemName.Name = "LBL_ItemName";
        LBL_ItemName.Size = new Size(203, 20);
        LBL_ItemName.TabIndex = 0;
        LBL_ItemName.Text = "Air";
        LBL_ItemName.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // PNL_ItemsBackground
        // 
        PNL_ItemsBackground.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
        PNL_ItemsBackground.BackColor = Color.Transparent;
        PNL_ItemsBackground.BorderColor = Color.FromArgb(103, 99, 99);
        PNL_ItemsBackground.BorderRadius = 5;
        PNL_ItemsBackground.BorderThickness = 2;
        PNL_ItemsBackground.Controls.Add(VSB_Main);
        PNL_ItemsBackground.Controls.Add(LST_Items);
        PNL_ItemsBackground.CustomizableEdges = customizableEdges5;
        PNL_ItemsBackground.FillColor = Color.FromArgb(58, 55, 55);
        PNL_ItemsBackground.Location = new Point(12, 155);
        PNL_ItemsBackground.Name = "PNL_ItemsBackground";
        PNL_ItemsBackground.ShadowDecoration.CustomizableEdges = customizableEdges6;
        PNL_ItemsBackground.Size = new Size(577, 287);
        PNL_ItemsBackground.TabIndex = 0;
        PNL_ItemsBackground.UseTransparentBackground = true;
        // 
        // VSB_Main
        // 
        VSB_Main.BackColor = Color.Transparent;
        VSB_Main.BindingContainer = LST_Items;
        VSB_Main.BorderRadius = 5;
        VSB_Main.FillColor = Color.FromArgb(80, 76, 76);
        VSB_Main.InUpdate = false;
        VSB_Main.LargeChange = 10;
        VSB_Main.Location = new Point(556, 3);
        VSB_Main.Name = "VSB_Main";
        VSB_Main.ScrollbarSize = 18;
        VSB_Main.Size = new Size(18, 281);
        VSB_Main.TabIndex = 1;
        VSB_Main.TabStop = false;
        VSB_Main.ThumbColor = Color.FromArgb(103, 99, 99);
        VSB_Main.ThumbSize = 5F;
        VSB_Main.ThumbStyle = Guna.UI2.WinForms.Enums.ThumbStyle.Inset;
        // 
        // BTN_Replace
        // 
        BTN_Replace.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        BTN_Replace.Animated = true;
        BTN_Replace.BackColor = Color.Transparent;
        BTN_Replace.BorderRadius = 5;
        BTN_Replace.Cursor = Cursors.Hand;
        BTN_Replace.CustomizableEdges = customizableEdges7;
        BTN_Replace.DisabledState.BorderColor = Color.FromArgb(80, 76, 76);
        BTN_Replace.DisabledState.CustomBorderColor = Color.FromArgb(80, 76, 76);
        BTN_Replace.DisabledState.FillColor = Color.FromArgb(80, 76, 76);
        BTN_Replace.DisabledState.ForeColor = Color.FromArgb(151, 148, 148);
        BTN_Replace.FillColor = Color.FromArgb(162, 123, 90);
        BTN_Replace.Font = new Font("Leelawadee UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        BTN_Replace.ForeColor = Color.White;
        BTN_Replace.Location = new Point(727, 490);
        BTN_Replace.Name = "BTN_Replace";
        BTN_Replace.ShadowDecoration.CustomizableEdges = customizableEdges8;
        BTN_Replace.Size = new Size(99, 32);
        BTN_Replace.TabIndex = 3;
        BTN_Replace.Text = "Replace";
        BTN_Replace.UseTransparentBackground = true;
        BTN_Replace.Click += BTN_Replace_Click;
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
        BTN_Cancel.DialogResult = DialogResult.Cancel;
        BTN_Cancel.DisabledState.BorderColor = Color.FromArgb(80, 76, 76);
        BTN_Cancel.DisabledState.CustomBorderColor = Color.FromArgb(80, 76, 76);
        BTN_Cancel.DisabledState.FillColor = Color.FromArgb(80, 76, 76);
        BTN_Cancel.DisabledState.ForeColor = Color.FromArgb(151, 148, 148);
        BTN_Cancel.FillColor = Color.FromArgb(80, 76, 76);
        BTN_Cancel.Font = new Font("Leelawadee UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        BTN_Cancel.ForeColor = Color.White;
        BTN_Cancel.HoverState.BorderColor = Color.FromArgb(103, 99, 99);
        BTN_Cancel.Location = new Point(12, 490);
        BTN_Cancel.Name = "BTN_Cancel";
        BTN_Cancel.ShadowDecoration.CustomizableEdges = customizableEdges10;
        BTN_Cancel.Size = new Size(119, 32);
        BTN_Cancel.TabIndex = 2;
        BTN_Cancel.Text = "Cancel";
        BTN_Cancel.UseTransparentBackground = true;
        BTN_Cancel.Click += BTN_Cancel_Click;
        // 
        // BTN_Reset
        // 
        BTN_Reset.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        BTN_Reset.Animated = true;
        BTN_Reset.BackColor = Color.Transparent;
        BTN_Reset.BorderColor = Color.FromArgb(80, 76, 76);
        BTN_Reset.BorderRadius = 5;
        BTN_Reset.BorderThickness = 2;
        BTN_Reset.Cursor = Cursors.Hand;
        BTN_Reset.CustomizableEdges = customizableEdges11;
        BTN_Reset.DialogResult = DialogResult.Cancel;
        BTN_Reset.DisabledState.BorderColor = Color.FromArgb(80, 76, 76);
        BTN_Reset.DisabledState.CustomBorderColor = Color.FromArgb(80, 76, 76);
        BTN_Reset.DisabledState.FillColor = Color.FromArgb(80, 76, 76);
        BTN_Reset.DisabledState.ForeColor = Color.FromArgb(151, 148, 148);
        BTN_Reset.FillColor = Color.FromArgb(80, 76, 76);
        BTN_Reset.Font = new Font("Leelawadee UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        BTN_Reset.ForeColor = Color.White;
        BTN_Reset.HoverState.BorderColor = Color.FromArgb(103, 99, 99);
        BTN_Reset.Location = new Point(137, 490);
        BTN_Reset.Name = "BTN_Reset";
        BTN_Reset.ShadowDecoration.CustomizableEdges = customizableEdges12;
        BTN_Reset.Size = new Size(201, 32);
        BTN_Reset.TabIndex = 2;
        BTN_Reset.Text = "Restore original material";
        BTN_Reset.UseTransparentBackground = true;
        BTN_Reset.Click += BTN_Reset_Click;
        // 
        // FRM_ReplaceMaterial
        // 
        AcceptButton = BTN_Replace;
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(38, 34, 34);
        CancelButton = BTN_Cancel;
        ClientSize = new Size(838, 534);
        Controls.Add(BTN_Reset);
        Controls.Add(BTN_Cancel);
        Controls.Add(BTN_Replace);
        Controls.Add(PNL_ItemsBackground);
        Controls.Add(PNL_Preview);
        Controls.Add(TBX_Search);
        Controls.Add(LBL_Description);
        Controls.Add(LBL_Heading);
        Font = new Font("Leelawadee UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        ForeColor = Color.FromArgb(235, 234, 234);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        Icon = (Icon)resources.GetObject("$this.Icon");
        Margin = new Padding(3, 4, 3, 4);
        MaximizeBox = false;
        Name = "FRM_ReplaceMaterial";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "FRM_ReplaceMaterial";
        Load += FRM_ReplaceMaterial_Load;
        PNL_Preview.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)PBX_Icon).EndInit();
        PNL_ItemsBackground.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label LBL_Heading;
    private Label LBL_Description;
    private Guna.UI2.WinForms.Guna2TextBox TBX_Search;
    private ListView LST_Items;
    private Guna.UI2.WinForms.Guna2Panel PNL_Preview;
    private Guna.UI2.WinForms.Guna2Panel PNL_ItemsBackground;
    private PictureBox PBX_Icon;
    private Label LBL_ItemName;
    private Label LBL_ItemId;
    private Label LBL_StackSize;
    private Guna.UI2.WinForms.Guna2Button BTN_Replace;
    private Guna.UI2.WinForms.Guna2Button BTN_Cancel;
    private Guna.UI2.WinForms.Guna2Button BTN_Reset;
    private Guna.UI2.WinForms.Guna2VScrollBar VSB_Main;
}