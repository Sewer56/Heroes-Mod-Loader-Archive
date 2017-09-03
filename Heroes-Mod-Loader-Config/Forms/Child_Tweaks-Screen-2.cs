using System;
using SonicHeroes.Variables;
using System.Windows.Forms;

namespace HeroesModLoaderConfig
{
    public partial class Tweaks_Screen_II : Form
    {
        /// <summary>
        /// By turning on compositing for the form, we can kill the flickering of dark control overdraws as we draw our own controls in an interactive fashion..
        /// </summary>
        protected override CreateParams CreateParams { get { CreateParams cp = base.CreateParams; cp.ExStyle |= 0x02000000; return cp; } }

        /// <summary>
        /// Constructor for the tweaks screen.
        /// </summary>
        public Tweaks_Screen_II()
        {
            InitializeComponent();
            CenterToScreen();
            this.BringToFront();
        }

        /// <summary>
        /// Load all of the relevant values for each setting when the tweaks screen is loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TweaksScreen_Shown(object sender, EventArgs e)
        {
            TinyUI_ComboBoxSmall_BGM.SelectedIndex = Program.ConfigFile.BGMToggle;
            TinyUI_ComboBoxSmall_SEVoice.SelectedIndex = Program.ConfigFile.SFXToggle;
            TinyUI_TrackBar_BGM.Value = Program.ConfigFile.BGMVolume;
            TinyUI_TrackBar_SEVoice.Value = Program.ConfigFile.SFXVolume;


        }

        private void TweaksScreen_Leave(object sender, EventArgs e)
        {
            
            Program.ConfigFile.BGMToggle = (byte)TinyUI_ComboBoxSmall_BGM.SelectedIndex;
            Program.ConfigFile.SFXToggle = (byte)TinyUI_ComboBoxSmall_SEVoice.SelectedIndex;
            Program.ConfigFile.BGMVolume = (byte)TinyUI_TrackBar_BGM.Value;
            Program.ConfigFile.SFXVolume = (byte)TinyUI_TrackBar_SEVoice.Value;
        }

        private void TinyUI_TrackBar_SEVoice_ValueChanged(object sender, EventArgs e)
        {
            TxtMini_TrackBarCounterSEVoice.Text = "SE/Voice Volume: " + Convert.ToString(TinyUI_TrackBar_SEVoice.Value);
        }

        private void TinyUI_TrackBar_BGM_ValueChanged(object sender, EventArgs e)
        {
            TxtMini_TrackBarCounterBGM.Text = "Background Music Volume: " + Convert.ToString(TinyUI_TrackBar_BGM.Value);
        }
    }
}
