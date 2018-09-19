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
using System.Reflection;

namespace HeroesModLoaderConfig
{
    public partial class AboutScreen : Form
    {
        /// <summary>
        /// By turning on compositing for the form, we can kill the flickering of dark control overdraws as we draw our own controls in an interactive fashion..
        /// </summary>
        protected override CreateParams CreateParams { get { CreateParams cp = base.CreateParams; cp.ExStyle |= 0x02000000; return cp; } }

        /// <summary>
        /// Constructor for the "About" Screen.
        /// </summary>
        public AboutScreen()
        {
            InitializeComponent();
            CenterToScreen();
            this.BringToFront();
        }

        /// <summary>
        /// Open a link to the Github source code.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnHyperlink_Github_Click(object sender, EventArgs e) { System.Diagnostics.Process.Start("https://github.com/sewer56lol/Heroes-Mod-Loader"); }
        /// <summary>
        /// Open a link to the SSRG Forum Thread.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnHyperlink_SSRG_Click(object sender, EventArgs e) { System.Diagnostics.Process.Start("https://www.youtube.com/watch?v=dQw4w9WgXcQ"); }
        /// <summary>
        /// Open a link to the Heroes Hacking Central Discord.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnHyperlink_HHC_Click(object sender, EventArgs e) { System.Diagnostics.Process.Start("https://discord.gg/4DHujrb"); }
    }
}
