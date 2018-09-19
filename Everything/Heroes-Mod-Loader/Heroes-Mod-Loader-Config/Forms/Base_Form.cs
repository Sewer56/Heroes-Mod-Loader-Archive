using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace HeroesModLoaderConfig
{
    public partial class MainFormSmall : Form
    {
        /// <summary>
        /// By turning on compositing for the form, we can kill the flickering of dark control overdraws as we draw our own controls in an interactive fashion..
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        /// <summary>
        /// Allow for the form to be dragged around.
        /// </summary>
        private void MainFormSmall_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                WinAPIComponents.ReleaseCapture();
                WinAPIComponents.SendMessage(Handle, WinAPIComponents.WM_NCLBUTTONDOWN, WinAPIComponents.HT_CAPTION, 0);
                this.Invalidate();
            }
        }

        /// <summary>
        /// Allow for movement if any of the bottom or top of form elements are moved and held.
        /// </summary>
        private void Panel_TitleBar_MouseMove(object sender, MouseEventArgs e) { MainFormSmall_MouseMove(sender, e); }
        private void Panel_SideBar_MouseMove(object sender, MouseEventArgs e) { MainFormSmall_MouseMove(sender, e); }
        private void Toolstrip_Bottom_MouseMove(object sender, MouseEventArgs e) { MainFormSmall_MouseMove(sender, e); }
        private void TinyUI_TopLabel_PageTitle_MouseMove(object sender, MouseEventArgs e) { MainFormSmall_MouseMove(sender, e); }

        /// <summary>
        /// Redirect the generic theme base controls that we are going to use to theme this application to the specific values used in this form.
        /// </summary>
        public void RedirectThemeValues()
        {
            Program.ToolstripThemeControlLabel = lbl_ActionBar_BottomRight;
            Program.SideBarPanel = Panel_SideBar;
            Program.TopBarPanel = Panel_TitleBar;
            Program.BottomToolstrip = Toolstrip_Bottom;
            Program.OpenedForms.Add(this);
        }

        /// 
        /// A bunch of stuff, including a copy of every form!
        ///                                                                                     
        public ThemeMenuTinyUI ThemeMenu;
        public AboutScreen AboutMenu;
        public MainScreen MainMenu;
        public ControllerScreen ControllerScreenOne;
        public ControllerScreenTwo ControllerScreenTwo;
        public TweaksScreen TweaksScreen;
        public Tweaks_Screen_II TweaksIIScreen;
        public Controller_Screen_Loader ModLoaderControllerScreen;

        /// <summary>
        /// The constructor for this class.
        /// </summary>
        public MainFormSmall()
        {
            InitializeComponent();
            CenterToScreen(); // Center to screen
            this.BringToFront(); // Bring to front
            Setup_Application(); // Set up form.
        }

        /// <summary>
        /// Sets up all of the aspects of the application which are governed by this form.
        /// </summary>
        public void Setup_Application()
        {
            // Set reference in Program to this being the main window.
            Program.xSmallMainWindow = this;
            // Do not draw an outline on toolstrips!
            Toolstrip_Bottom.Renderer = new MyToolStrip();
            // Set the region of this form to be a rectangle such that the application gains rounded edges.
            this.Region = System.Drawing.Region.FromHrgn(WinAPIComponents.CreateRoundRectRgn(0, 0, this.Width, this.Height, 30, 30));
            // Set the theme controls from which other controls will inherit their colour and theme properties.
            RedirectThemeValues();
            // Automatic theming of all assets in this form.
            ThemeMethods.DoThemeAssets(this);
            // Create an instance of each form, theme the form, add it to the list of opened form and set the Multiple Document Interface parent.
            SetupSwappableScreens();
            // Automatically load the application theme.
            ThemeMethods.AutoLoadCurrentTheme();
            // Shows the default menu.
            Show_Default_Menu();
            // If the game is not Sonic Heroes, hide all irrelevant menus.
            if (!Program.Game_Is_Sonic_Heroes) { Hide_NonSonicHeroes(); }
        }

        /// <summary>
        /// Hides all of the non-Sonic-Heroes menus if the game is not Sonic Heroes.
        /// </summary>
        public void Hide_NonSonicHeroes()
        {
            SideBtn_Tweaks.Visible = false;
            SideBtn_TweaksII.Visible = false;
            SideBtn_ControllerOne.Visible = false;
            SideBtn_ControllerTwo.Visible = false;
            SideBtn_LauncherSeparator_II.Visible = false;
            lbl_ActionBar_BottomRight.Text = "Non Sonic-Heroes/Generic Game Mode!";
        }

        /// <summary>
        /// Creates an instance of each form, theme the form, add it to the list of opened form and set the Multiple Document Interface parent.
        /// </summary>
        public void SetupSwappableScreens()
        {
            this.IsMdiContainer = true;

            ThemeMenu = new ThemeMenuTinyUI();
            ThemeMethods.DoThemeAssets(ThemeMenu);
            Program.OpenedForms.Add(ThemeMenu);
            ThemeMenu.MdiParent = this;

            MainMenu = new MainScreen();
            ThemeMethods.DoThemeAssets(MainMenu);
            Program.OpenedForms.Add(MainMenu);
            MainMenu.MdiParent = this;

            ControllerScreenOne = new ControllerScreen();
            ThemeMethods.DoThemeAssets(ControllerScreenOne);
            Program.OpenedForms.Add(ControllerScreenOne);
            ControllerScreenOne.MdiParent = this;

            TweaksScreen = new TweaksScreen();
            ThemeMethods.DoThemeAssets(TweaksScreen);
            Program.OpenedForms.Add(TweaksScreen);
            TweaksScreen.MdiParent = this;

            TweaksIIScreen = new Tweaks_Screen_II();
            ThemeMethods.DoThemeAssets(TweaksIIScreen);
            Program.OpenedForms.Add(TweaksIIScreen);
            TweaksIIScreen.MdiParent = this;

            ControllerScreenTwo = new ControllerScreenTwo();
            ThemeMethods.DoThemeAssets(ControllerScreenTwo);
            Program.OpenedForms.Add(ControllerScreenTwo);
            ControllerScreenTwo.MdiParent = this;

            AboutMenu = new AboutScreen();
            ThemeMethods.DoThemeAssets(AboutMenu);
            Program.OpenedForms.Add(AboutMenu);
            AboutMenu.MdiParent = this;

            ModLoaderControllerScreen = new Controller_Screen_Loader();
            ThemeMethods.DoThemeAssets(ModLoaderControllerScreen);
            Program.OpenedForms.Add(ModLoaderControllerScreen);
            ModLoaderControllerScreen.MdiParent = this;

            ThemeMDIClients();
        }

        /// <summary>
        /// Hide the currenly opened form.
        /// </summary>
        private void SetupNewSwappableForm() // Setup for new form to be displayed.
        {
            try { Program.CurrentlyOpenedForm.Hide(); } catch { }
            // Will always throw on first running, yesm this is a bit dirty.
        }

        /// <summary>
        /// Set properties of each MDI container form to allow it to correctly display.
        /// </summary>
        private void FinishSwappableFormSetup()
        {
            Program.CurrentlyOpenedForm.Visible = true;
            Program.CurrentlyOpenedForm.Location = new Point(0, 0);
            Program.CurrentlyOpenedForm.Dock = DockStyle.Fill;
        }

        /// <summary>
        /// Apply correct theming to each page/MDI client.
        /// </summary>
        private void ThemeMDIClients()
        {
            foreach (MdiClient Control in this.Controls.OfType<MdiClient>())
            {
                // Set the BackColor of the MdiClient control.
                Control.BackColor = this.BackColor;
                Control.MouseMove += MainFormSmall_MouseMove;
                WinAPIComponents.SetBevel(this, false);
            }
        }

        /// 
        /// The methods below are responsible for loading each child form.
        /// 

        private void SideBtn_Themes_Click(object sender, EventArgs e)
        {
            TinyUI_TopLabel_PageTitle.Text = "Theming Menu";
            SetupNewSwappableForm();
            Program.CurrentlyOpenedForm = ThemeMenu;
            FinishSwappableFormSetup();
        }
        
        private void SideBtn_LoaderInputStack_Click(object sender, EventArgs e)
        {
            TinyUI_TopLabel_PageTitle.Text = "Mod Loader: Input Stack";
            SetupNewSwappableForm();
            Program.CurrentlyOpenedForm = ModLoaderControllerScreen;
            FinishSwappableFormSetup();
        }

        private void SideBtn_Tweaks_Click(object sender, EventArgs e)
        {
            TinyUI_TopLabel_PageTitle.Text = "Tweaks Menu";
            SetupNewSwappableForm();
            Program.CurrentlyOpenedForm = TweaksScreen;
            FinishSwappableFormSetup();
        }

        private void SideBtn_TweaksII_Click(object sender, EventArgs e)
        {
            TinyUI_TopLabel_PageTitle.Text = "Tweaks Menu II";
            SetupNewSwappableForm();
            Program.CurrentlyOpenedForm = TweaksIIScreen;
            FinishSwappableFormSetup();
        }

        private void SideBtn_ControllerOne_Click(object sender, EventArgs e)
        {
            TinyUI_TopLabel_PageTitle.Text = "Controller I Buttons";
            SetupNewSwappableForm();
            Program.CurrentlyOpenedForm = ControllerScreenOne;
            FinishSwappableFormSetup();
        }

        private void SideBtn_ControllerTwo_Click(object sender, EventArgs e)
        {
            TinyUI_TopLabel_PageTitle.Text = "Controller II Buttons";
            SetupNewSwappableForm();
            Program.CurrentlyOpenedForm = ControllerScreenTwo;
            FinishSwappableFormSetup();
        }

        private void SideBtn_About_Click(object sender, EventArgs e)
        {
            TinyUI_TopLabel_PageTitle.Text = "About Mod Loader";
            SetupNewSwappableForm();
            Program.CurrentlyOpenedForm = AboutMenu;
            FinishSwappableFormSetup();
        }


        public void SideBtn_MainScreen_Click(object sender, EventArgs e)
        {
            TinyUI_TopLabel_PageTitle.Text = "Main Menu";
            SetupNewSwappableForm();
            Program.CurrentlyOpenedForm = MainMenu;
            FinishSwappableFormSetup();
        }

        /// <summary>
        /// Show the default menu to be opened.
        /// </summary>
        private void Show_Default_Menu()
        {
            SetupNewSwappableForm();
            Program.CurrentlyOpenedForm = MainMenu;
            FinishSwappableFormSetup();
        }

        /// <summary>
        /// Changes the text of the bottom right bar.
        /// </summary>
        /// <param name="Message"></param>
        public void ActionBar_Label_Change_Text(string Message) { lbl_ActionBar_BottomRight.Text = Message; }

    }
}
