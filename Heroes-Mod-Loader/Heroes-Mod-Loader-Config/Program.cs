using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SonicHeroes.Misc;
using SonicHeroes.Controller;
using System.IO;

namespace HeroesModLoaderConfig
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static Fonts xFonts = new Fonts();
        public static MainFormSmall xSmallMainWindow;
        public static string Executable_Path; // The path of the game executable.
        public static string Root_Directory; // The directory of the game root.
        public static string Mod_Loader_Directory; // The directory of the mod loader.
        public static string Mod_Loader_Backup_Directory; // The directory of backup game files.

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
        public static SonicHeroes.Controller.DirectInput_Joystick_Manager Controller_Handle = new SonicHeroes.Controller.DirectInput_Joystick_Manager();

        /// <summary>
        /// Sonic Heroes specific, if the Mod Loader is in Sonic Heroes mode.
        /// </summary>
        public static Sonic_Heroes_Program Sonic_Heroes_Specific_Stuff;
        public static bool Game_Is_Sonic_Heroes; // Flag which controls whether the game we are modding is Sonic Heroes.

        [STAThread]
        static void Main()
        {
            //Application.SetCompatibleTextRenderingDefault(false); // Windows likes to mess with application styling :/
            Mod_Loader_Directory = AppDomain.CurrentDomain.BaseDirectory; // The directory containing the mod loader itself.

            // Set up Mod Loader for first time use if necessary.
            if (!File.Exists(Mod_Loader_Directory + @"Mod-Loader-Config\Setup_Complete")) { Run_Mod_Loader_Setup(); }
            Setup_Directories();

            // Check if the game is Sonic Heroes
            string Sonic_Heroes_Status = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"Mod-Loader-Config\Setup_Complete");
            if (Sonic_Heroes_Status == "SonicHeroes") { Game_Is_Sonic_Heroes = true; Sonic_Heroes_Specific_Stuff = new Sonic_Heroes_Program(); } else { Game_Is_Sonic_Heroes = false; }

            xFonts.SetupFonts(); // Get the fonts up and running!
            xSmallMainWindow = new MainFormSmall(); // Loading occurs here in the BG.
            Application.Run(xSmallMainWindow); // Let's start!
        }

        /// <summary>
        /// Sets up the mod loader for first use.
        /// </summary>
        static void Run_Mod_Loader_Setup()
        {
            // Welcome
            MessageBox.Show("Welcome to the Preview Release of the Heroes Mod Loader!\n\nIt would appear that this is your first time running the configurator.\n\nPlease follow the instructions onscreen in order to set up the mod loader, you will only be required to do this once.\n\nNote: To re-run this setup at any time, simply remove the file 'Setup_Complete' in folder 'Mod-Loader-Config'", "Woohoo! It's a Beta!",MessageBoxButtons.OK,MessageBoxIcon.Information);

            // Create all necessary directories and files.
            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"Mod-Loader-Config");
            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"Mod-Loader-Backup");
            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"Mod-Loader-Libraries");
            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"Mod-Loader-Mods");
            File.Create(AppDomain.CurrentDomain.BaseDirectory + @"Mod-Loader-Config\Enabled_Mods.txt").Close(); 
            File.Create(AppDomain.CurrentDomain.BaseDirectory + @"Mod-Loader-Config\Executable_Path.txt").Close(); 
            File.Create(AppDomain.CurrentDomain.BaseDirectory + @"Mod-Loader-Config\Root_Directory.txt").Close(); 

            // Sonic Heroes Question
            DialogResult Sonic_Heroes_Dialogue_Result = MessageBox.Show("Is the game which you are planning to run the mod loader with Sonic Heroes?\n\n(The Mod Loader has an universal implementation and should work with a multitude of games, selecting 'no' will disable any Sonic Heroes specific features)","Sonikku Hirozu?",MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (Sonic_Heroes_Dialogue_Result == DialogResult.Yes) { Game_Is_Sonic_Heroes = true; } else { Game_Is_Sonic_Heroes = false; }
            
            // Set Executable Path
            MessageBox.Show("Please navigate to and select your game's executable you wish to use the Mod Loader with.\n\ne.g. `D:/Projects/Heroes_HAX/Sonic_Heroes/Tsonic_win.exe`\nNote: Next window after may sometimes appear under other windows.");
            System.Windows.Forms.OpenFileDialog SonicHeroes_Executable_Dialog = new OpenFileDialog();
            SonicHeroes_Executable_Dialog.Title = "Please set path to game's main executable.";
            SonicHeroes_Executable_Dialog.Filter = "Executable Files|*.exe";
            SonicHeroes_Executable_Dialog.InitialDirectory = @"C:\";
            SonicHeroes_Executable_Dialog.Multiselect = false;

            OpenDialog: // The user should press OK, hopefully...
            DialogResult Game_Executable_Dialogue_Result = SonicHeroes_Executable_Dialog.ShowDialog();
            if (Game_Executable_Dialogue_Result == DialogResult.OK) { Executable_Path = SonicHeroes_Executable_Dialog.FileName; }
            else { MessageBox.Show("This message is a placeholder for a witty comment I will insert here one day."); goto OpenDialog; }

            // Set Directory Path
            MessageBox.Show("Please navigate to and select your game's root directory you wish to use the Mod Loader with.\n\ne.g. `D:/Projects/Heroes_HAX/Sonic_Heroes/`");
            System.Windows.Forms.FolderBrowserDialog SonicHeroes_Directory_Dialog = new FolderBrowserDialog();

            OpenDialog_II: // The user should press OK, hopefully...
            DialogResult Game_Directory_Dialogue_Result = SonicHeroes_Directory_Dialog.ShowDialog();
            if (Game_Directory_Dialogue_Result == DialogResult.OK) { Root_Directory = SonicHeroes_Directory_Dialog.SelectedPath; }
            else { MessageBox.Show("Placeholders are fun."); goto OpenDialog_II; }

            // Write Completed Setup File
            if (Game_Is_Sonic_Heroes) { File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"Mod-Loader-Config\Setup_Complete", "SonicHeroes"); }
            else { File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"Mod-Loader-Config\Setup_Complete", "NotSonicHeroes"); }
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"Mod-Loader-Config\Executable_Path.txt", Executable_Path);
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"Mod-Loader-Config\Root_Directory.txt", Root_Directory);
        }

        /// Reads the directories to be used by the mod loader from the text files.
        static void Setup_Directories()
        {
            Executable_Path = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"Mod-Loader-Config\Executable_Path.txt"); // Main directory for Sonic Heroes
            Root_Directory = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"Mod-Loader-Config\Root_Directory.txt"); // Main directory for Sonic Heroes
            Mod_Loader_Backup_Directory = AppDomain.CurrentDomain.BaseDirectory + @"Mod-Loader-Backup\"; // Backup directory for Sonic Heroes
        }
    }
}
