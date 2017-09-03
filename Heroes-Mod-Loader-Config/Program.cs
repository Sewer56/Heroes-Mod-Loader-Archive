using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SonicHeroes.Misc;
using SonicHeroes.Controller;

namespace HeroesModLoaderConfig
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static Fonts xFonts = new Fonts();
        public static MainFormSmall xSmallMainWindow;

        /// <summary>
        /// Automatic redirect properties for application theming
        /// </summary>
        public static ToolStripLabel ToolstripThemeControlLabel;
        public static Panel SideBarPanel;
        public static Panel TopBarPanel;
        public static ToolStrip BottomToolstrip;

        /// <summary>
        /// Form management for the MDI individual child forms.
        /// </summary>
        public static List<Form> OpenedForms = new List<Form>();
        public static Form CurrentlyOpenedForm;

        /// <summary>
        /// Store the Sonic Heroes Configuration File
        /// </summary>
        public static SonicHeroes.Misc.SonicHeroes_Miscallenous.Sonic_Heroes_Configuration_File ConfigFile = SonicHeroes.Misc.SonicHeroes_Miscallenous.Load_Configuration_File();
        public static byte[] SonicHeroesExecutable = SonicHeroes.Misc.SonicHeroes_Miscallenous.Get_SonicHeroes_Executable_As_Array();
        public static SonicHeroes.Controller.DirectInput_Joystick_Manager Controller_Handle = new SonicHeroes.Controller.DirectInput_Joystick_Manager();


        [STAThread]
        static void Main()
        {
            xFonts.SetupFonts(); // Get the fonts up and running!
            Application.SetCompatibleTextRenderingDefault(false); // Windows likes to mess with application styling :/
            xSmallMainWindow = new MainFormSmall(); // Loading occurs here in the BG.
            Application.Run(xSmallMainWindow); // Let's start!
        }
    }
}
