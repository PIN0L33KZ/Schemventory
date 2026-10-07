using Schemventory.App;

namespace Schemventory.Forms;

public partial class FRM_About : Form
{
    private const string WindowName = $"About - {Constants.AppName}";

    public FRM_About()
    {
        InitializeComponent();

        Text = WindowName;

        LBL_ProgramName.Text = $"About {Constants.AppName}";
        LBL_CopyrightText.Text = $"{Constants.AppName} Version {Application.ProductVersion}\nCopyright © {Constants.AppAuthor}, {DateTime.Now.Year}\n{Constants.AppWebsite}";
        LBL_3rdPartyText.Text =
            @"This software uses icons provided by Icons8.
https://icons8.com

Minecraft item metadata and related game data are sourced from misode/mcmeta.
https://github.com/misode/mcmeta

Minecraft item and block renders are provided by blockrender.dev.
https://blockrender.dev

Minecraft textures © Mojang Studios. Render by blockrender.dev.

This software is an independent project and is not an official Minecraft product. It is not approved by, endorsed by, or associated with Mojang Studios or Microsoft.

Minecraft is a trademark of Mojang AB.";
    }
}