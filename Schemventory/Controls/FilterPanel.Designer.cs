namespace Schemventory.Controls;

partial class FilterPanel
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
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
        PNL_Background = new Guna.UI2.WinForms.Guna2Panel();
        CHP_Amount = new Guna.UI2.WinForms.Guna2Chip();
        CHB_Value = new Guna.UI2.WinForms.Guna2CheckBox();
        PNL_Background.SuspendLayout();
        SuspendLayout();
        // 
        // PNL_Background
        // 
        PNL_Background.BorderColor = Color.FromArgb(217, 74, 74);
        PNL_Background.BorderRadius = 5;
        PNL_Background.BorderThickness = 2;
        PNL_Background.Controls.Add(CHP_Amount);
        PNL_Background.Controls.Add(CHB_Value);
        PNL_Background.CustomizableEdges = customizableEdges3;
        PNL_Background.Dock = DockStyle.Fill;
        PNL_Background.FillColor = Color.FromArgb(64, 47, 48);
        PNL_Background.Location = new Point(0, 0);
        PNL_Background.Name = "PNL_Background";
        PNL_Background.ShadowDecoration.CustomizableEdges = customizableEdges4;
        PNL_Background.Size = new Size(166, 47);
        PNL_Background.TabIndex = 0;
        // 
        // CHP_Amount
        // 
        CHP_Amount.Anchor = AnchorStyles.Right;
        CHP_Amount.AutoRoundedCorners = true;
        CHP_Amount.BorderColor = Color.Empty;
        CHP_Amount.CustomizableEdges = customizableEdges1;
        CHP_Amount.FillColor = Color.FromArgb(176, 78, 77);
        CHP_Amount.Font = new Font("Leelawadee UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        CHP_Amount.ForeColor = Color.White;
        CHP_Amount.IsClosable = false;
        CHP_Amount.Location = new Point(114, 11);
        CHP_Amount.Name = "CHP_Amount";
        CHP_Amount.ShadowDecoration.CustomizableEdges = customizableEdges2;
        CHP_Amount.Size = new Size(42, 25);
        CHP_Amount.TabIndex = 0;
        CHP_Amount.TabStop = false;
        CHP_Amount.Text = "000";
        // 
        // CHB_Value
        // 
        CHB_Value.Anchor = AnchorStyles.Left;
        CHB_Value.Animated = true;
        CHB_Value.AutoSize = true;
        CHB_Value.CheckedState.BorderColor = Color.FromArgb(217, 74, 74);
        CHB_Value.CheckedState.BorderRadius = 2;
        CHB_Value.CheckedState.BorderThickness = 0;
        CHB_Value.CheckedState.FillColor = Color.FromArgb(217, 74, 74);
        CHB_Value.Cursor = Cursors.Hand;
        CHB_Value.Font = new Font("Leelawadee UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        CHB_Value.Location = new Point(9, 11);
        CHB_Value.Name = "CHB_Value";
        CHB_Value.Size = new Size(78, 24);
        CHB_Value.TabIndex = 0;
        CHB_Value.Text = "Missing";
        CHB_Value.UncheckedState.BorderColor = Color.FromArgb(103, 99, 99);
        CHB_Value.UncheckedState.BorderRadius = 2;
        CHB_Value.UncheckedState.BorderThickness = 0;
        CHB_Value.UncheckedState.FillColor = Color.FromArgb(103, 99, 99);
        CHB_Value.CheckedChanged += CHB_Value_CheckedChanged;
        // 
        // FilterPanel
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.Transparent;
        Controls.Add(PNL_Background);
        Font = new Font("Leelawadee UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        ForeColor = Color.FromArgb(235, 234, 234);
        Margin = new Padding(3, 4, 3, 4);
        Name = "FilterPanel";
        Size = new Size(166, 47);
        PNL_Background.ResumeLayout(false);
        PNL_Background.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private Guna.UI2.WinForms.Guna2Panel PNL_Background;
    private Guna.UI2.WinForms.Guna2CheckBox CHB_Value;
    private Guna.UI2.WinForms.Guna2Chip CHP_Amount;
}
