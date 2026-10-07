namespace Schemventory.Forms;

partial class FRM_About
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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FRM_About));
        LBL_ProgramName = new Label();
        LBL_CopirightHeading = new Label();
        LBL_CopyrightText = new Label();
        LBL_3rdPartyHeading = new Label();
        LBL_3rdPartyText = new Label();
        PBX_AppLogo = new PictureBox();
        ((System.ComponentModel.ISupportInitialize)PBX_AppLogo).BeginInit();
        SuspendLayout();
        // 
        // LBL_ProgramName
        // 
        LBL_ProgramName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        LBL_ProgramName.AutoEllipsis = true;
        LBL_ProgramName.Font = new Font("Leelawadee UI", 20.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        LBL_ProgramName.ForeColor = Color.FromArgb(162, 123, 90);
        LBL_ProgramName.Location = new Point(12, 9);
        LBL_ProgramName.Name = "LBL_ProgramName";
        LBL_ProgramName.Size = new Size(463, 37);
        LBL_ProgramName.TabIndex = 0;
        LBL_ProgramName.Text = "Heading";
        // 
        // LBL_CopirightHeading
        // 
        LBL_CopirightHeading.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        LBL_CopirightHeading.AutoEllipsis = true;
        LBL_CopirightHeading.Font = new Font("Leelawadee UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
        LBL_CopirightHeading.ForeColor = Color.FromArgb(194, 165, 142);
        LBL_CopirightHeading.Location = new Point(12, 59);
        LBL_CopirightHeading.Name = "LBL_CopirightHeading";
        LBL_CopirightHeading.Size = new Size(463, 21);
        LBL_CopirightHeading.TabIndex = 0;
        LBL_CopirightHeading.Text = "Copyright";
        // 
        // LBL_CopyrightText
        // 
        LBL_CopyrightText.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        LBL_CopyrightText.AutoEllipsis = true;
        LBL_CopyrightText.Font = new Font("Leelawadee UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        LBL_CopyrightText.ForeColor = Color.FromArgb(235, 234, 234);
        LBL_CopyrightText.Location = new Point(12, 92);
        LBL_CopyrightText.Name = "LBL_CopyrightText";
        LBL_CopyrightText.Size = new Size(463, 65);
        LBL_CopyrightText.TabIndex = 0;
        LBL_CopyrightText.Text = "Text\r\nText2\r\nText3";
        // 
        // LBL_3rdPartyHeading
        // 
        LBL_3rdPartyHeading.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        LBL_3rdPartyHeading.AutoEllipsis = true;
        LBL_3rdPartyHeading.Font = new Font("Leelawadee UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
        LBL_3rdPartyHeading.ForeColor = Color.FromArgb(194, 165, 142);
        LBL_3rdPartyHeading.Location = new Point(12, 176);
        LBL_3rdPartyHeading.Name = "LBL_3rdPartyHeading";
        LBL_3rdPartyHeading.Size = new Size(569, 21);
        LBL_3rdPartyHeading.TabIndex = 0;
        LBL_3rdPartyHeading.Text = "Third-Party Credits";
        // 
        // LBL_3rdPartyText
        // 
        LBL_3rdPartyText.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        LBL_3rdPartyText.AutoEllipsis = true;
        LBL_3rdPartyText.Font = new Font("Leelawadee UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        LBL_3rdPartyText.ForeColor = Color.FromArgb(235, 234, 234);
        LBL_3rdPartyText.Location = new Point(12, 209);
        LBL_3rdPartyText.Name = "LBL_3rdPartyText";
        LBL_3rdPartyText.Size = new Size(569, 301);
        LBL_3rdPartyText.TabIndex = 0;
        LBL_3rdPartyText.Text = resources.GetString("LBL_3rdPartyText.Text");
        // 
        // PBX_AppLogo
        // 
        PBX_AppLogo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        PBX_AppLogo.Image = Properties.Resources.AppLogo;
        PBX_AppLogo.Location = new Point(481, 12);
        PBX_AppLogo.Name = "PBX_AppLogo";
        PBX_AppLogo.Size = new Size(100, 100);
        PBX_AppLogo.SizeMode = PictureBoxSizeMode.AutoSize;
        PBX_AppLogo.TabIndex = 4;
        PBX_AppLogo.TabStop = false;
        // 
        // FRM_About
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(38, 34, 34);
        ClientSize = new Size(593, 519);
        Controls.Add(PBX_AppLogo);
        Controls.Add(LBL_3rdPartyText);
        Controls.Add(LBL_CopyrightText);
        Controls.Add(LBL_3rdPartyHeading);
        Controls.Add(LBL_CopirightHeading);
        Controls.Add(LBL_ProgramName);
        Font = new Font("Leelawadee UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        ForeColor = Color.FromArgb(235, 234, 234);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        Icon = (Icon)resources.GetObject("$this.Icon");
        Margin = new Padding(3, 4, 3, 4);
        MaximizeBox = false;
        Name = "FRM_About";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "FRM_About";
        ((System.ComponentModel.ISupportInitialize)PBX_AppLogo).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label LBL_ProgramName;
    private Label LBL_CopirightHeading;
    private Label LBL_CopyrightText;
    private Label LBL_3rdPartyHeading;
    private Label LBL_3rdPartyText;
    private PictureBox PBX_AppLogo;
}