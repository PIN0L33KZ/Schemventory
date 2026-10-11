using Guna.UI2.WinForms;
using Serilog;
using Schemventory.App;

namespace Schemventory.Forms;

public partial class FRM_MaterialList
{
    private Guna2Button BTN_AddMaterial = null!;

    private void InitializeAddMaterialControls()
    {
        BTN_AddMaterial = new Guna2Button
        {
            Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
            Animated = true,
            BackColor = Color.Transparent,
            BorderRadius = 5,
            Cursor = Cursors.Hand,
            FillColor = Color.FromArgb(162, 123, 90),
            Font = new Font("Leelawadee UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0),
            ForeColor = Color.White,
            Location = new Point(827, 495),
            Name = "BTN_AddMaterial",
            Size = new Size(131, 30),
            TabIndex = 5,
            Text = "Add material",
            UseTransparentBackground = true
        };

        BTN_AddMaterial.DisabledState.BorderColor = Color.FromArgb(80, 76, 76);
        BTN_AddMaterial.DisabledState.CustomBorderColor = Color.FromArgb(80, 76, 76);
        BTN_AddMaterial.DisabledState.FillColor = Color.FromArgb(80, 76, 76);
        BTN_AddMaterial.DisabledState.ForeColor = Color.FromArgb(151, 148, 148);

        BTN_AddMaterial.Click += BTN_AddMaterial_Click;

        Controls.Add(BTN_AddMaterial);
        BTN_AddMaterial.BringToFront();

        TTP_Main.SetToolTip(BTN_AddMaterial, "Add a material to this project");
    }

    private void BTN_AddMaterial_Click(object? sender, EventArgs e)
    {
        Log.Debug("{LogContext} Add material dialog requested. ProjectId={ProjectId}", LogContext, _project.Id);

        using FRM_AddMaterial addMaterialForm = new(_itemDataProvider, _itemIconService);

        if(addMaterialForm.ShowDialog(this) != DialogResult.OK || string.IsNullOrWhiteSpace(addMaterialForm.SelectedItemId))
        {
            Log.Debug("{LogContext} Add material dialog cancelled. ProjectId={ProjectId}", LogContext, _project.Id);
            return;
        }

        try
        {
            _projectMaterialService.Add(_project.Id, addMaterialForm.SelectedItemId, addMaterialForm.RequiredAmount);

            LoadMaterialList();
            ApplyMaterialView(resetScrollPosition: false);
            UpdateStatistics();

            Log.Information("{LogContext} Material added manually. ProjectId={ProjectId}, ItemId={ItemId}, RequiredAmount={RequiredAmount}", LogContext, _project.Id, addMaterialForm.SelectedItemId, addMaterialForm.RequiredAmount);
        }
        catch(Exception exception)
        {
            Log.Error(exception, "{LogContext} Failed to add material manually. ProjectId={ProjectId}, ItemId={ItemId}", LogContext, _project.Id, addMaterialForm.SelectedItemId);

            _ = MessageBox.Show(
                $"The material could not be added.\n\n{exception.Message}",
                Constants.AppName,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
