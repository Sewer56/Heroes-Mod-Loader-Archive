using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SonicHeroes.Variables;
using System.Reflection;
using System.Diagnostics;

namespace HeroesModLoaderConfig
{
    public partial class TweaksScreen : Form
    {
        /// <summary>
        /// By turning on compositing for the form, we can kill the flickering of dark control overdraws as we draw our own controls in an interactive fashion..
        /// </summary>
        protected override CreateParams CreateParams { get { CreateParams cp = base.CreateParams; cp.ExStyle |= 0x02000000; return cp; } }

        public TweaksScreen()
        {
            InitializeComponent();
            CenterToScreen();
            this.BringToFront();
        }

        /// <summary>
        /// When the screen is shown, load each of the values from the configuration file into the appropriate fields.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainScreen_Shown(object sender, EventArgs e)
        {
            int Width = BitConverter.ToInt32(Program.Sonic_Heroes_Specific_Stuff.SonicHeroesExecutable, (int)SonicHeroesVariables.Launcher_Addresses.Width_StockLauncher_1280_1024);
            int Height = BitConverter.ToInt32(Program.Sonic_Heroes_Specific_Stuff.SonicHeroesExecutable, (int)SonicHeroesVariables.Launcher_Addresses.Height_StockLauncher_1280_1024);
            TinyUI_TxtBoxSmaller_ResolutionWidth.Text = Width + "x" + Height;
            TinyUI_ComboBoxSmall_AnisotropicFilter.SelectedIndex = Program.Sonic_Heroes_Specific_Stuff.ConfigFile.AnisotropicFiltering;
            TinyUI_ComboBoxSmall_FogEmulation.SelectedIndex = Program.Sonic_Heroes_Specific_Stuff.ConfigFile.FogEmulation;
            TinyUI_ComboBoxSmall_SoftShadows.SelectedIndex = Program.Sonic_Heroes_Specific_Stuff.ConfigFile.SoftShadows;
            TinyUI_ComboBoxSmall_Fullscreen.SelectedIndex = Program.Sonic_Heroes_Specific_Stuff.ConfigFile.FullScreen;
            TinyUI_ComboBoxSmall_Language.SelectedIndex = Program.Sonic_Heroes_Specific_Stuff.ConfigFile.Language;
            TinyUI_ComboBoxSmall_ClippingSetting.SelectedIndex = Program.Sonic_Heroes_Specific_Stuff.ConfigFile.ClipRange;
            TinyUI_ComboBoxSmall_FrameRate.SelectedIndex = Program.Sonic_Heroes_Specific_Stuff.ConfigFile.FrameRate;
            TinyUI_ComboBoxSmall_SurroundSound.SelectedIndex = Program.Sonic_Heroes_Specific_Stuff.ConfigFile.SurroundSound;

            switch (BitConverter.ToInt32(Program.Sonic_Heroes_Specific_Stuff.SonicHeroesExecutable, (int)SonicHeroesVariables.Launcher_Addresses.Window_Style))
            {
                case (Int32)SonicHeroesVariables.WINAPI_BorderStyles.Stock:
                    TinyUI_ComboBoxSmall_BorderStyle.SelectedIndex = 0;
                    break;
                case (Int32)SonicHeroesVariables.WINAPI_BorderStyles.Borderless:
                    TinyUI_ComboBoxSmall_BorderStyle.SelectedIndex = 1;
                    break;
                case (Int32)SonicHeroesVariables.WINAPI_BorderStyles.Resizable:
                    TinyUI_ComboBoxSmall_BorderStyle.SelectedIndex = 2;
                    break;
                case (Int32)SonicHeroesVariables.WINAPI_BorderStyles.Resizable_Borderless:
                    TinyUI_ComboBoxSmall_BorderStyle.SelectedIndex = 3;
                    break;
            }
        }

        /// <summary>
        /// Validate and set the user placed custom resolution.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TinyUI_TxtBoxSmaller_ResolutionWidth_Leave(object sender, EventArgs e)
        {
            try
            {
                int Width = Convert.ToInt32(TinyUI_TxtBoxSmaller_ResolutionWidth.Text.Substring(0, TinyUI_TxtBoxSmaller_ResolutionWidth.Text.IndexOf("x")));
                int Height = Convert.ToInt32(TinyUI_TxtBoxSmaller_ResolutionWidth.Text.Substring(TinyUI_TxtBoxSmaller_ResolutionWidth.Text.IndexOf("x") + 1));
                Buffer.BlockCopy(BitConverter.GetBytes(Width), 0, Program.Sonic_Heroes_Specific_Stuff.SonicHeroesExecutable, (int)SonicHeroesVariables.Launcher_Addresses.Width_StockLauncher_1280_1024, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(Width), 0, Program.Sonic_Heroes_Specific_Stuff.SonicHeroesExecutable, (int)SonicHeroesVariables.Launcher_Addresses.Width_StockLauncher_FullScreen_1280_1024, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(Height), 0, Program.Sonic_Heroes_Specific_Stuff.SonicHeroesExecutable, (int)SonicHeroesVariables.Launcher_Addresses.Height_StockLauncher_1280_1024, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(Height), 0, Program.Sonic_Heroes_Specific_Stuff.SonicHeroesExecutable, (int)SonicHeroesVariables.Launcher_Addresses.Height_StockLauncher_FullScreen_1280_1024, 4);
            }
            catch { MessageBox.Show("Invalid Resolution! It has been reset to 1920x1080."); TinyUI_TxtBoxSmaller_ResolutionWidth.Text = "1920x1080"; }
        }

        /// <summary>
        /// Save each variable as we leave the screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TweaksScreen_Leave(object sender, EventArgs e)
        {
            Program.Sonic_Heroes_Specific_Stuff.ConfigFile.AnisotropicFiltering = (byte)TinyUI_ComboBoxSmall_AnisotropicFilter.SelectedIndex;
            Program.Sonic_Heroes_Specific_Stuff.ConfigFile.FogEmulation = (byte)TinyUI_ComboBoxSmall_FogEmulation.SelectedIndex;
            Program.Sonic_Heroes_Specific_Stuff.ConfigFile.SoftShadows = (byte)TinyUI_ComboBoxSmall_SoftShadows.SelectedIndex;
            Program.Sonic_Heroes_Specific_Stuff.ConfigFile.FullScreen = (byte)TinyUI_ComboBoxSmall_Fullscreen.SelectedIndex;
            Program.Sonic_Heroes_Specific_Stuff.ConfigFile.ClipRange = (byte)TinyUI_ComboBoxSmall_ClippingSetting.SelectedIndex;
            Program.Sonic_Heroes_Specific_Stuff.ConfigFile.FrameRate = (byte)TinyUI_ComboBoxSmall_FrameRate.SelectedIndex;
            Program.Sonic_Heroes_Specific_Stuff.ConfigFile.Language = (byte)TinyUI_ComboBoxSmall_Language.SelectedIndex;
            Program.Sonic_Heroes_Specific_Stuff.ConfigFile.SurroundSound = (byte)TinyUI_ComboBoxSmall_SurroundSound.SelectedIndex;

            switch (TinyUI_ComboBoxSmall_BorderStyle.SelectedIndex)
            {
                case 0:
                    Buffer.BlockCopy(BitConverter.GetBytes((Int32)SonicHeroesVariables.WINAPI_BorderStyles.Stock), 0, Program.Sonic_Heroes_Specific_Stuff.SonicHeroesExecutable, (int)SonicHeroesVariables.Launcher_Addresses.Window_Style, 4);
                    Buffer.BlockCopy(BitConverter.GetBytes((Int32)SonicHeroesVariables.WINAPI_BorderStyles.Stock), 0, Program.Sonic_Heroes_Specific_Stuff.SonicHeroesExecutable, (int)SonicHeroesVariables.Launcher_Addresses.Adjust_Window_Rect_Style, 4);
                    break;
                case 1:
                    Buffer.BlockCopy(BitConverter.GetBytes((Int32)SonicHeroesVariables.WINAPI_BorderStyles.Borderless), 0, Program.Sonic_Heroes_Specific_Stuff.SonicHeroesExecutable, (int)SonicHeroesVariables.Launcher_Addresses.Window_Style, 4);
                    Buffer.BlockCopy(BitConverter.GetBytes((Int32)SonicHeroesVariables.WINAPI_BorderStyles.Borderless), 0, Program.Sonic_Heroes_Specific_Stuff.SonicHeroesExecutable, (int)SonicHeroesVariables.Launcher_Addresses.Adjust_Window_Rect_Style, 4);
                    break;
                case 2:
                    Buffer.BlockCopy(BitConverter.GetBytes((Int32)SonicHeroesVariables.WINAPI_BorderStyles.Resizable), 0, Program.Sonic_Heroes_Specific_Stuff.SonicHeroesExecutable, (int)SonicHeroesVariables.Launcher_Addresses.Window_Style, 4);
                    Buffer.BlockCopy(BitConverter.GetBytes((Int32)SonicHeroesVariables.WINAPI_BorderStyles.Resizable), 0, Program.Sonic_Heroes_Specific_Stuff.SonicHeroesExecutable, (int)SonicHeroesVariables.Launcher_Addresses.Adjust_Window_Rect_Style, 4);
                    break;
                case 3:
                    Buffer.BlockCopy(BitConverter.GetBytes((Int32)SonicHeroesVariables.WINAPI_BorderStyles.Resizable_Borderless), 0, Program.Sonic_Heroes_Specific_Stuff.SonicHeroesExecutable, (int)SonicHeroesVariables.Launcher_Addresses.Window_Style, 4);
                    Buffer.BlockCopy(BitConverter.GetBytes((Int32)SonicHeroesVariables.WINAPI_BorderStyles.Resizable_Borderless), 0, Program.Sonic_Heroes_Specific_Stuff.SonicHeroesExecutable, (int)SonicHeroesVariables.Launcher_Addresses.Adjust_Window_Rect_Style, 4);
                    break;
            }
        }

    }
}
