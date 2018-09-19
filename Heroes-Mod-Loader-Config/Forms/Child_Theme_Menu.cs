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
    public partial class ThemeMenuTinyUI : Form
    {
        /// <summary>
        /// By turning on compositing for the form, we can kill the flickering of dark control overdraws as we draw our own controls in an interactive fashion..
        /// </summary>
        protected override CreateParams CreateParams { get { CreateParams cp = base.CreateParams; cp.ExStyle |= 0x02000000; return cp; } }


        public ThemeMenuTinyUI()
        {
            InitializeComponent();
            CenterToScreen();
            this.BringToFront();
        }

        private void Btn_Load_Click(object sender, EventArgs e) { ThemeMethods.LoadCurrentTheme(); }
        private void Btn_Save_Click(object sender, EventArgs e) { ThemeMethods.SaveCurrentTheme(); }

        /// The magic that handles the theming is in ThemeMethods.cs, that includes automatic subscribing to colour changing for buttons based off of names etc.
        
    }
}
