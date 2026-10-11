namespace Schemventory.Forms;

partial class FRM_AddMaterial
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if(disposing && components != null)
            components.Dispose();

        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges9 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges10 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges11 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges12 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges13 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FRM_AddMaterial));
        LBL_Heading = new Label();
        LBL_Description = new Label();
        TBX_Search = new Guna.UI2.WinForms.Guna2TextBox();
        PNL_ItemsBackground = new Guna.UI2.WinForms.Guna2Panel();
        VSB_Main = new Guna.UI2.WinForms.Guna2VScrollBar();
        LST_Items = new ListView();
        PNL_Preview = new Guna.UI2.WinForms.Guna2Panel();
        PGI_Loading = new Guna.UI2.WinForms.Guna2WinProgressIndicator();
        LBL_AmountPreview = new Label();
        NUD_RequiredAmount = new Guna.UI2.WinForms.Guna2NumericUpDown();
        LBL_RequiredAmount = new Label();
        LBL_StackSize = new Label();
        LBL_ItemId = new Label();
        LBL_ItemName = new Label();
        PBX_Icon = new PictureBox();
        BTN_Add = new Guna.UI2.WinForms.Guna2Button();
        BTN_Cancel = new Guna.UI2.WinForms.Guna2Button();
        PNL_ItemsBackground.SuspendLayout();
        PNL_Preview.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)NUD_RequiredAmount).BeginInit();
        ((System.ComponentModel.ISupportInitialize)PBX_Icon).BeginInit();
        SuspendLayout();
        // 
        // LBL_Heading
        // 
        LBL_Heading.AutoSize = true;
        LBL_Heading.Font = new Font("Leelawadee UI", 20.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        LBL_Heading.ForeColor = Color.FromArgb(162, 123, 90);
        LBL_Heading.Location = new Point(12, 9);
        LBL_Heading.Name = "LBL_Heading";
        LBL_Heading.Size = new Size(185, 37);
        LBL_Heading.TabIndex = 0;
        LBL_Heading.Text = "Add material";
        // 
        // LBL_Description
        // 
        LBL_Description.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        LBL_Description.Font = new Font("Leelawadee UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
        LBL_Description.ForeColor = Color.FromArgb(194, 165, 142);
        LBL_Description.Location = new Point(12, 55);
        LBL_Description.Name = "LBL_Description";
        LBL_Description.Size = new Size(814, 25);
        LBL_Description.TabIndex = 0;
        LBL_Description.Text = "Choose a material and enter the required amount.";
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
        // PNL_ItemsBackground
        // 
        PNL_ItemsBackground.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        PNL_ItemsBackground.BackColor = Color.Transparent;
        PNL_ItemsBackground.BorderColor = Color.FromArgb(103, 99, 99);
        PNL_ItemsBackground.BorderRadius = 5;
        PNL_ItemsBackground.BorderThickness = 2;
        PNL_ItemsBackground.Controls.Add(VSB_Main);
        PNL_ItemsBackground.Controls.Add(LST_Items);
        PNL_ItemsBackground.CustomizableEdges = customizableEdges3;
        PNL_ItemsBackground.FillColor = Color.FromArgb(58, 55, 55);
        PNL_ItemsBackground.Location = new Point(12, 155);
        PNL_ItemsBackground.Name = "PNL_ItemsBackground";
        PNL_ItemsBackground.Padding = new Padding(3);
        PNL_ItemsBackground.ShadowDecoration.CustomizableEdges = customizableEdges4;
        PNL_ItemsBackground.Size = new Size(576, 287);
        PNL_ItemsBackground.TabIndex = 2;
        // 
        // VSB_Main
        // 
        VSB_Main.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
        VSB_Main.BackColor = Color.Transparent;
        VSB_Main.BindingContainer = LST_Items;
        VSB_Main.BorderRadius = 5;
        VSB_Main.FillColor = Color.FromArgb(80, 76, 76);
        VSB_Main.InUpdate = false;
        VSB_Main.LargeChange = 10;
        VSB_Main.Location = new Point(555, 3);
        VSB_Main.Name = "VSB_Main";
        VSB_Main.ScrollbarSize = 18;
        VSB_Main.Size = new Size(18, 281);
        VSB_Main.TabIndex = 1;
        VSB_Main.TabStop = false;
        VSB_Main.ThumbColor = Color.FromArgb(103, 99, 99);
        VSB_Main.ThumbSize = 5F;
        VSB_Main.ThumbStyle = Guna.UI2.WinForms.Enums.ThumbStyle.Inset;
        // 
        // LST_Items
        // 
        LST_Items.BackColor = Color.FromArgb(58, 55, 55);
        LST_Items.BorderStyle = BorderStyle.None;
        LST_Items.Dock = DockStyle.Fill;
        LST_Items.ForeColor = Color.FromArgb(235, 234, 234);
        LST_Items.FullRowSelect = true;
        LST_Items.HeaderStyle = ColumnHeaderStyle.None;
        LST_Items.Location = new Point(3, 3);
        LST_Items.MultiSelect = false;
        LST_Items.Name = "LST_Items";
        LST_Items.Size = new Size(570, 281);
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
        PNL_Preview.Controls.Add(PGI_Loading);
        PNL_Preview.Controls.Add(LBL_AmountPreview);
        PNL_Preview.Controls.Add(NUD_RequiredAmount);
        PNL_Preview.Controls.Add(LBL_RequiredAmount);
        PNL_Preview.Controls.Add(LBL_StackSize);
        PNL_Preview.Controls.Add(LBL_ItemId);
        PNL_Preview.Controls.Add(LBL_ItemName);
        PNL_Preview.Controls.Add(PBX_Icon);
        PNL_Preview.CustomizableEdges = customizableEdges8;
        PNL_Preview.FillColor = Color.FromArgb(58, 55, 55);
        PNL_Preview.Location = new Point(594, 155);
        PNL_Preview.Name = "PNL_Preview";
        PNL_Preview.ShadowDecoration.CustomizableEdges = customizableEdges9;
        PNL_Preview.Size = new Size(231, 287);
        PNL_Preview.TabIndex = 3;
        // 
        // PGI_Loading
        // 
        PGI_Loading.AutoStart = true;
        PGI_Loading.BackColor = Color.Transparent;
        PGI_Loading.CircleSize = 1F;
        PGI_Loading.Location = new Point(84, 48);
        PGI_Loading.Name = "PGI_Loading";
        PGI_Loading.ProgressColor = Color.FromArgb(162, 123, 90);
        PGI_Loading.ShadowDecoration.CustomizableEdges = customizableEdges5;
        PGI_Loading.Size = new Size(64, 64);
        PGI_Loading.TabIndex = 4;
        PGI_Loading.TabStop = false;
        PGI_Loading.UseTransparentBackground = true;
        PGI_Loading.Visible = false;
        // 
        // LBL_AmountPreview
        // 
        LBL_AmountPreview.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        LBL_AmountPreview.AutoEllipsis = true;
        LBL_AmountPreview.ForeColor = Color.FromArgb(194, 165, 142);
        LBL_AmountPreview.Location = new Point(17, 264);
        LBL_AmountPreview.Name = "LBL_AmountPreview";
        LBL_AmountPreview.Size = new Size(197, 20);
        LBL_AmountPreview.TabIndex = 0;
        LBL_AmountPreview.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // NUD_RequiredAmount
        // 
        NUD_RequiredAmount.BackColor = Color.Transparent;
        NUD_RequiredAmount.BorderColor = Color.FromArgb(103, 99, 99);
        NUD_RequiredAmount.BorderRadius = 5;
        NUD_RequiredAmount.BorderThickness = 2;
        NUD_RequiredAmount.CustomizableEdges = customizableEdges6;
        NUD_RequiredAmount.FillColor = Color.FromArgb(58, 55, 55);
        NUD_RequiredAmount.FocusedState.BorderColor = Color.FromArgb(162, 123, 90);
        NUD_RequiredAmount.Font = new Font("Leelawadee UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        NUD_RequiredAmount.ForeColor = Color.FromArgb(235, 234, 234);
        NUD_RequiredAmount.Location = new Point(60, 218);
        NUD_RequiredAmount.Maximum = new decimal(new int[] { 999999999, 0, 0, 0 });
        NUD_RequiredAmount.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        NUD_RequiredAmount.Name = "NUD_RequiredAmount";
        NUD_RequiredAmount.ShadowDecoration.CustomizableEdges = customizableEdges7;
        NUD_RequiredAmount.Size = new Size(111, 36);
        NUD_RequiredAmount.TabIndex = 1;
        NUD_RequiredAmount.UpDownButtonFillColor = Color.FromArgb(162, 123, 90);
        NUD_RequiredAmount.UpDownButtonForeColor = Color.FromArgb(235, 234, 234);
        NUD_RequiredAmount.Value = new decimal(new int[] { 1, 0, 0, 0 });
        NUD_RequiredAmount.ValueChanged += NUD_RequiredAmount_ValueChanged;
        // 
        // LBL_RequiredAmount
        // 
        LBL_RequiredAmount.AutoSize = true;
        LBL_RequiredAmount.Font = new Font("Leelawadee UI", 11.25F);
        LBL_RequiredAmount.ForeColor = Color.FromArgb(235, 234, 234);
        LBL_RequiredAmount.Location = new Point(83, 195);
        LBL_RequiredAmount.Name = "LBL_RequiredAmount";
        LBL_RequiredAmount.Size = new Size(65, 20);
        LBL_RequiredAmount.TabIndex = 0;
        LBL_RequiredAmount.Text = "Quantity";
        // 
        // LBL_StackSize
        // 
        LBL_StackSize.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        LBL_StackSize.AutoEllipsis = true;
        LBL_StackSize.Font = new Font("Leelawadee UI", 9F);
        LBL_StackSize.ForeColor = Color.FromArgb(151, 148, 148);
        LBL_StackSize.Location = new Point(10, 164);
        LBL_StackSize.Name = "LBL_StackSize";
        LBL_StackSize.Size = new Size(211, 14);
        LBL_StackSize.TabIndex = 0;
        LBL_StackSize.Text = "Stack size: 0";
        LBL_StackSize.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // LBL_ItemId
        // 
        LBL_ItemId.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        LBL_ItemId.AutoEllipsis = true;
        LBL_ItemId.Font = new Font("Leelawadee UI", 9F);
        LBL_ItemId.ForeColor = Color.FromArgb(151, 148, 148);
        LBL_ItemId.Location = new Point(10, 150);
        LBL_ItemId.Name = "LBL_ItemId";
        LBL_ItemId.Size = new Size(211, 14);
        LBL_ItemId.TabIndex = 0;
        LBL_ItemId.Text = "minecraft:empty";
        LBL_ItemId.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // LBL_ItemName
        // 
        LBL_ItemName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        LBL_ItemName.AutoEllipsis = true;
        LBL_ItemName.Font = new Font("Leelawadee UI", 11.25F);
        LBL_ItemName.ForeColor = Color.FromArgb(235, 234, 234);
        LBL_ItemName.Location = new Point(10, 123);
        LBL_ItemName.Name = "LBL_ItemName";
        LBL_ItemName.Size = new Size(211, 23);
        LBL_ItemName.TabIndex = 0;
        LBL_ItemName.Text = "No material selected";
        LBL_ItemName.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // PBX_Icon
        // 
        PBX_Icon.Location = new Point(84, 48);
        PBX_Icon.Name = "PBX_Icon";
        PBX_Icon.Size = new Size(64, 64);
        PBX_Icon.SizeMode = PictureBoxSizeMode.Zoom;
        PBX_Icon.TabIndex = 0;
        PBX_Icon.TabStop = false;
        // 
        // BTN_Add
        // 
        BTN_Add.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        BTN_Add.Animated = true;
        BTN_Add.BackColor = Color.Transparent;
        BTN_Add.BorderRadius = 5;
        BTN_Add.Cursor = Cursors.Hand;
        BTN_Add.CustomizableEdges = customizableEdges10;
        BTN_Add.DisabledState.BorderColor = Color.FromArgb(80, 76, 76);
        BTN_Add.DisabledState.CustomBorderColor = Color.FromArgb(80, 76, 76);
        BTN_Add.DisabledState.FillColor = Color.FromArgb(80, 76, 76);
        BTN_Add.DisabledState.ForeColor = Color.FromArgb(151, 148, 148);
        BTN_Add.FillColor = Color.FromArgb(162, 123, 90);
        BTN_Add.Font = new Font("Leelawadee UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        BTN_Add.ForeColor = Color.White;
        BTN_Add.Location = new Point(739, 490);
        BTN_Add.Name = "BTN_Add";
        BTN_Add.ShadowDecoration.CustomizableEdges = customizableEdges11;
        BTN_Add.Size = new Size(87, 32);
        BTN_Add.TabIndex = 3;
        BTN_Add.Text = "Add";
        BTN_Add.UseTransparentBackground = true;
        BTN_Add.Click += BTN_Add_Click;
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
        BTN_Cancel.CustomizableEdges = customizableEdges12;
        BTN_Cancel.DisabledState.BorderColor = Color.FromArgb(80, 76, 76);
        BTN_Cancel.DisabledState.CustomBorderColor = Color.FromArgb(80, 76, 76);
        BTN_Cancel.DisabledState.FillColor = Color.FromArgb(80, 76, 76);
        BTN_Cancel.DisabledState.ForeColor = Color.FromArgb(151, 148, 148);
        BTN_Cancel.FillColor = Color.FromArgb(80, 76, 76);
        BTN_Cancel.Font = new Font("Leelawadee UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        BTN_Cancel.ForeColor = Color.White;
        BTN_Cancel.Location = new Point(12, 490);
        BTN_Cancel.Name = "BTN_Cancel";
        BTN_Cancel.ShadowDecoration.CustomizableEdges = customizableEdges13;
        BTN_Cancel.Size = new Size(119, 32);
        BTN_Cancel.TabIndex = 2;
        BTN_Cancel.Text = "Cancel";
        BTN_Cancel.UseTransparentBackground = true;
        BTN_Cancel.Click += BTN_Cancel_Click;
        // 
        // FRM_AddMaterial
        // 
        AcceptButton = BTN_Add;
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(38, 34, 34);
        CancelButton = BTN_Cancel;
        ClientSize = new Size(838, 534);
        Controls.Add(BTN_Cancel);
        Controls.Add(BTN_Add);
        Controls.Add(PNL_Preview);
        Controls.Add(PNL_ItemsBackground);
        Controls.Add(TBX_Search);
        Controls.Add(LBL_Description);
        Controls.Add(LBL_Heading);
        DoubleBuffered = true;
        Font = new Font("Leelawadee UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        ForeColor = Color.FromArgb(235, 234, 234);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        Icon = (Icon)resources.GetObject("$this.Icon");
        Margin = new Padding(3, 4, 3, 4);
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "FRM_AddMaterial";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "FRM_AddMaterial";
        Load += FRM_AddMaterial_Load;
        PNL_ItemsBackground.ResumeLayout(false);
        PNL_Preview.ResumeLayout(false);
        PNL_Preview.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)NUD_RequiredAmount).EndInit();
        ((System.ComponentModel.ISupportInitialize)PBX_Icon).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label LBL_Heading;
    private Label LBL_Description;
    private Guna.UI2.WinForms.Guna2TextBox TBX_Search;
    private Guna.UI2.WinForms.Guna2Panel PNL_ItemsBackground;
    private ListView LST_Items;
    private Guna.UI2.WinForms.Guna2Panel PNL_Preview;
    private PictureBox PBX_Icon;
    private Label LBL_ItemName;
    private Label LBL_ItemId;
    private Label LBL_StackSize;
    private Label LBL_RequiredAmount;
    private Guna.UI2.WinForms.Guna2NumericUpDown NUD_RequiredAmount;
    private Label LBL_AmountPreview;
    private Guna.UI2.WinForms.Guna2Button BTN_Add;
    private Guna.UI2.WinForms.Guna2Button BTN_Cancel;
    private Guna.UI2.WinForms.Guna2VScrollBar VSB_Main;
    private Guna.UI2.WinForms.Guna2WinProgressIndicator PGI_Loading;
}
